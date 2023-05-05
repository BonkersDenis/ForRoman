using System;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        private GeometryModel3D mGeometry; // геометрическая модель, которую будем рисовать
        private bool mDown;// переменная для прос=верки нажатия левой клавиши мыши
        private Point mLastPos;// переменная необходимая для работы с камерой

        public MainWindow()
        {
            InitializeComponent();
            BuildSolid();
        }

        private void BuildSolid()
        {
            MeshGeometry3D mesh = new MeshGeometry3D(); // создаём сетку на основе которой будет созданна модель
            // добавляем вершины сетки
            mesh.Positions.Add(new Point3D(-1, -1, 1)); // 0
            mesh.Positions.Add(new Point3D(1, -1, 1)); // 1
            mesh.Positions.Add(new Point3D(1, 1, 1)); // 2
            mesh.Positions.Add(new Point3D(-1, 1, 1)); // 3
            mesh.Positions.Add(new Point3D(-1, -1, -1)); // 4
            mesh.Positions.Add(new Point3D(1, -1, -1)); // 5
            mesh.Positions.Add(new Point3D(1, 1, -1)); // 6
            mesh.Positions.Add(new Point3D(-1, 1, -1)); // 7

            // создаём стороны куба из треугольников
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2); // спереди
           // mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(6); // справа
            mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(6); // сверху
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(1); // снизу
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(7); // слева
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(5); // сзади
            mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(4);


            mGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Red)); // созфём модель из сетки 
            mGeometry.Transform = new Transform3DGroup();// создаём трансформацию для нашей модели
            group.Children.Add(mGeometry);// групируем наши модели
        }
        // отделение камеры при прокручивании колёсика мыши
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - e.Delta / 500D);
        }
        // вращение нашей модели при нажатой левой кнопки мыши и передвижении
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDown)
            {
                Point pos = Mouse.GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - mLastPos.X, dy = actualPos.Y - mLastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0) mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0) mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                Transform3DGroup group = mGeometry.Transform as Transform3DGroup;
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                group.Children.Add(new RotateTransform3D(r));
                mLastPos = actualPos;
            }
        }

        // обработка нажатия мыши
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            mDown = true;
            Point pos = Mouse.GetPosition(viewport);
            mLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
        }
        // обработка прекращения нажатия мыши
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
        }
    }
}
