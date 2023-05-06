import os
import telegram
from telegram.ext import Updater, CommandHandler, MessageHandler
import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
from sklearn.neighbors import NearestNeighbors
from scipy.sparse import csr_matrix
from imdb import IMDb
import csv
import numpy as np
import telebot





movies = pd.read_csv('movies.csv')
rating = pd.read_csv('ratings.csv')
movies.drop(['genres'], axis = 1, inplace = True)
movies.head(3)
rating.drop(['timestamp'], axis = 1, inplace = True)
rating.head(3)

user_item_matrix = rating.pivot(index = 'movieId', columns = 'userId', values = 'rating')
user_item_matrix.head()

# параметр inplace = True опять же поможет сохранить результат
user_item_matrix.fillna(0, inplace = True)
user_item_matrix.head()
# вначале сгруппируем (объединим) пользователей, возьмем только столбец rating
# и посчитаем, сколько было оценок у каждого пользователя
users_votes = rating.groupby('userId')['rating'].agg('count')

# сделаем то же самое, только для фильма
movies_votes = rating.groupby('movieId')['rating'].agg('count')

# теперь создадим фильтр (mask)
user_mask = users_votes[users_votes > 50].index
movie_mask = movies_votes[movies_votes > 10].index

# применим фильтры и отберем фильмы с достаточным количеством оценок
user_item_matrix = user_item_matrix.loc[movie_mask, :]

# а также активных пользователей
user_item_matrix = user_item_matrix.loc[:, user_mask]

csr_data = csr_matrix(user_item_matrix.values)

user_item_matrix = user_item_matrix.rename_axis(None, axis = 1).reset_index()
user_item_matrix.head()

knn = NearestNeighbors(metric='cosine', algorithm='brute', n_neighbors=20, n_jobs=-1)

# обучим модель
knn.fit(csr_data)


# Создание экземпляра бота с помощью токена
bot = telebot.TeleBot("сдесь должен быть токен ")



@bot.message_handler(commands=['start'])
def send_welcome(message):
    # Отправка пользователю три плашки с кнопками
    keyboard = telebot.types.InlineKeyboardMarkup()
    button1 = telebot.types.InlineKeyboardButton(text="Фильм",callback_data="press" )
    button2 = telebot.types.InlineKeyboardButton(text="Сериал",callback_data="press1" )
    button3 = telebot.types.InlineKeyboardButton(text="Аниме", callback_data="press2")
    keyboard.add(button1, button2, button3)
    bot.send_message(message.chat.id, "Выберите что хотите посмотреть ", reply_markup=keyboard)

@bot.callback_query_handler(func=lambda call: True)
def callback_query(call):
    if call.data == 'press':
        bot.send_message(chat_id=call.message.chat.id, text='Привет! Я рекомендательный бот для фильмов и сериалов. Введите название понравившего вам фильма:')
        bot.register_next_step_handler_by_chat_id(call.message.chat.id, process_movie_input)
    elif call.data == 'press1':
        bot.send_message(chat_id=call.message.chat.id,text='Привет! Я рекомендательный бот для фильмов и сериалов. Введите название сериала:')
    elif call.data == 'press2':
        bot.send_message(chat_id=call.message.chat.id,text='Привет! Я рекомендательный бот для фильмов и сериалов. Введите название аниме:')
        bot.register_next_step_handler_by_chat_id(call.message.chat.id, process_anime_input)


@bot.message_handler(content_types=['text'], commands=None)
def process_movie_input(message):
    title = message.text
    recommendations = 10
    search_word = title
    movie_search = movies[movies['title'].str.contains(search_word)]
    movie_id = movie_search.iloc[0]['movieId']

    # далее по индексу фильма в датасете movies найдем соответствующий индекс
    # в матрице предпочтений
    movie_id = user_item_matrix[user_item_matrix['movieId'] == movie_id].index[0]
    distances, indices = knn.kneighbors(csr_data[movie_id], n_neighbors=recommendations + 1)
    indices_list = indices.squeeze().tolist()
    distances_list = distances.squeeze().tolist()

    # далее с помощью функций zip и list преобразуем наши списки
    indices_distances = list(zip(indices_list, distances_list))

    # остается отсортировать список по расстояниям через key = lambda x: x[1] (то есть по второму элементу)
    # в возрастающем порядке reverse = False
    indices_distances_sorted = sorted(indices_distances, key=lambda x: x[1], reverse=False)

    # и убрать первый элемент с индексом 901 (потому что это и есть "Матрица")
    indices_distances_sorted = indices_distances_sorted[1:]


    # создаем пустой список, в который будем помещать название фильма и расстояние до него
    recom_list = []

    # теперь в цикле будем поочередно проходить по кортежам
    for ind_dist in indices_distances_sorted:
        # искать movieId в матрице предпочтений
        matrix_movie_id = user_item_matrix.iloc[ind_dist[0]]['movieId']

        # выяснять индекс этого фильма в датафрейме movies
        id = movies[movies['movieId'] == matrix_movie_id].index

        # брать название фильма и расстояние до него
        title = movies.iloc[id]['title'].values[0]
        dist = ind_dist[1]

        # помещать каждую пару в питоновский словарь
        # который, в свою очередь, станет элементом списка recom_list
        recom_list.append({'Title': title, 'Distance': dist})
    recom_df = pd.DataFrame(recom_list, index=range(1, recommendations + 1), columns=["Title"])
    bot.send_message(message.chat.id, 'Рекомендуемые фильмы:\n' + '\n'.join(recom_df['Title'].tolist()))

@bot.message_handler(content_types=['text'], commands=None)
def process_anime_input(message):
    title = message.text


# Запуск бота
bot.polling()
