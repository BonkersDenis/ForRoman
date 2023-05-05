using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Praktik1;

public partial class EmployeeRegistrationWindow : Window
{
    private ValidateEmployeeRegistrationTextBoxes _validateTextBoxes;

    private string _directoryPath = @"C:\Users\voron\OneDrive\Рабочий стол\BudukovPraktika\FinalDirectory";
    private string _filePath = @"C:\Users\voron\OneDrive\Рабочий стол\BudukovPraktika\FinalDirectory\employee.txt";

    public EmployeeRegistrationWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        InitializeComponent();

        _validateTextBoxes = new ValidateEmployeeRegistrationTextBoxes();
        DataContext = _validateTextBoxes;
        ChangeEnableRegisterUserButton();
    }

    private void ChangeEnableRegisterUserButton() =>
        registerUserButton.IsEnabled = _validateTextBoxes.IsAllInputCorrect() ? true : false;

    private void registerUser_Click(object sender, RoutedEventArgs e)
    {
        ChangeEnableRegisterUserButton();
        FillFile();
    }

    private void backButton_Click(object sender, RoutedEventArgs e) => OpenAuthorizationWindow();

    private void OpenAuthorizationWindow()
    {
        AuthorizationWindow authorizationWindow = new AuthorizationWindow();
        authorizationWindow.Show();
        Close();
    }

    private void FillFile()
    {
        try
        {
            Directory.CreateDirectory(_directoryPath);

            if (IsIndexAlreadyRegistred(idTextBox.Text))
            {
                File.AppendAllText(_filePath, "ID: " + idTextBox.Text + " ");
                File.AppendAllText(_filePath, "Surname: " + surnameTextBox.Text + " ");
                File.AppendAllText(_filePath, "Name: " + nameTextBox.Text + " ");
                File.AppendAllText(_filePath, "Patronymic: " + patronymicTextBox.Text + " ");
                File.AppendAllText(_filePath, "Passport: " + passportTextBox.Text + " ");
                File.AppendAllText(_filePath, "Email: " + emailTextBox.Text + " \n");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private bool IsIndexAlreadyRegistred(string index)
    {
        string[] employeFileContent = new string[] { };

        try
        {
            if (!File.Exists(_filePath))
                return true;

            employeFileContent = File.ReadAllLines(_filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        foreach (string line in employeFileContent)
            if (line.Split()[1] == index)
            {
                MessageBox.Show("Employee with this ID is already registred!");
                idTextBox.BorderBrush = Brushes.Purple;
                return false;
            }
        idTextBox.BorderBrush = Brushes.Green;
        return true;
    }

    private void AnyTextChange(object sender, System.Windows.Input.MouseEventArgs e) =>
        ChangeEnableRegisterUserButton();
}