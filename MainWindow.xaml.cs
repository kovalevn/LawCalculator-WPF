using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OfficeOpenXml;

namespace LawCalculator_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            InitializeComponent();
            DataContext = new LCViewModel();

            //Подписываемся на события
            SearchBox.TextChanged += SearchBox_TextChanged;
            ShowToPayBox.Checked += ShowToPayBox_Checked;
            ShowToPayBox.Unchecked += ShowToPayBox_Unchecked;
        }

        private void ShowToPayBox_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ProjectsControl.Items.Count; i++)
            {
                ((UIElement)ProjectsControl.ItemContainerGenerator.ContainerFromIndex(i)).Visibility = Visibility.Visible;
            }
        }

        private void ShowToPayBox_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ProjectsControl.Items.Count; i++)
            {
                bool hide = true;
                foreach (Payment payment in (ProjectsControl.Items[i] as Project).Payments) if (payment.ToPay) hide = false;
                UIElement uiElement = (UIElement)ProjectsControl.ItemContainerGenerator.ContainerFromIndex(i);
                if (hide) uiElement.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = 0; i < ProjectsControl.Items.Count; i++)
            {
                UIElement uiElement =
                    (UIElement)ProjectsControl.ItemContainerGenerator.ContainerFromIndex(i);
                if (!(ProjectsControl.Items[i] as Project).Name.ToLower().Contains(SearchBox.Text.ToLower())) uiElement.Visibility = Visibility.Collapsed;
                else uiElement.Visibility = Visibility.Visible;
                if (SearchBox.Text == string.Empty) uiElement.Visibility = Visibility.Visible;
            }
        }

        //private void MakeProjects()
        //{
        //    foreach (Project project in AllProjects)
        //    {
        //        project.CountMoneyOld();

        //        Expander newProject = new Expander { Header = project.Name, Margin = new Thickness (0,0,0,5), DataContext = project };
        //        ProjectsPanel.Children.Add(newProject);
        //        GroupBox projectGroupBox = new GroupBox() { HorizontalAlignment = HorizontalAlignment.Left };
        //        StackPanel newProjectPanel = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left };
        //        projectGroupBox.Content = newProjectPanel;
        //        newProject.Content = projectGroupBox;

        //        //Создание и заполнение полей для Originating partner
        //        newProjectPanel.Children.Add(new TextBox()
        //        {
        //            Text = "Originator",
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            BorderThickness = new Thickness(0, 0, 0, 1),
        //            BorderBrush = Brushes.Black,
        //            IsReadOnly = true,
        //            Background = new SolidColorBrush() { Opacity = 0.3, Color = Colors.LightGray }
        //        });

        //        UniformGrid origPartGrid = new UniformGrid() { Margin = new Thickness(0,0,0,20) };
        //        origPartGrid.Columns = 3;
        //        //origPartGrid.Orientation = Orientation.Horizontal;
        //        //origPartGrid.HorizontalAlignment = HorizontalAlignment.Center;

        //        origPartGrid.Children.Add(new Label { Content = "Originating Partner" });
        //        origPartGrid.Children.Add(new Label { Content = "Проценты" });
        //        origPartGrid.Children.Add(new Label { Content = "К выплате" });

        //        //newProjectPanel.Children.Add(origPartGrid);

        //        //StackPanel origPartGrid = new StackPanel();
        //        //origPartGrid.Orientation = Orientation.Horizontal;
        //        //origPartGrid.HorizontalAlignment = HorizontalAlignment.Center;

        //        ComboBox origPartBox = new ComboBox
        //        {
        //            //origPartBox.VerticalAlignment = VerticalAlignment.Center;
        //            ItemsSource = AllPartners,
        //            DisplayMemberPath = "Name"
        //        };
        //        origPartBox.SetBinding(ComboBox.SelectedValueProperty, new Binding() { Source = project, Path = new PropertyPath("OriginatingPartner"), Mode = BindingMode.TwoWay });
        //        origPartGrid.Children.Add(origPartBox);

        //        TextBox origPartPercent = new TextBox() { Text = "20", MaxLength = 2, IsReadOnly = true };
        //        origPartGrid.Children.Add(origPartPercent);

        //        ComboBox origPartMoneyBox = new ComboBox() { SelectedIndex = 0 };
        //        origPartMoneyBox.ItemsSource = new ObservableCollection<string>()
        //        {
        //            $"{project.MoneyDollar * int.Parse(origPartPercent.Text) / 100} $",
        //            $"{project.MoneyRouble * int.Parse(origPartPercent.Text) / 100} ₽",
        //            $"{project.MoneyEuro * int.Parse(origPartPercent.Text) / 100} €",
        //            $"{project.MoneyDollarCashless * int.Parse(origPartPercent.Text) / 100} $ б/н",
        //            $"{project.MoneyRoubleCashless * int.Parse(origPartPercent.Text) / 100} ₽ б/н",
        //            $"{project.MoneyEuroCashless * int.Parse(origPartPercent.Text) / 100} € б/н",
        //        };

        //        //StackPanel origPartMoneyPanel = new StackPanel();

        //        //origPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyDollar * int.Parse(origPartPercent.Text) / 100} $", IsReadOnly = true });
        //        //origPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyRouble * int.Parse(origPartPercent.Text) / 100} Р", IsReadOnly = true });
        //        //origPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyDollarCashless * int.Parse(origPartPercent.Text) / 100} $ б/н", IsReadOnly = true });
        //        //origPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyRoubleCashless * int.Parse(origPartPercent.Text) / 100} Р б/н", IsReadOnly = true });

        //        origPartGrid.Children.Add(origPartMoneyBox);

        //        foreach (FrameworkElement el in origPartGrid.Children)
        //        {
        //            el.Margin = new Thickness(5);
        //        }

        //        newProjectPanel.Children.Add(origPartGrid);

        //        //Создание и заполнение полей для Managing partner
        //        newProjectPanel.Children.Add(new TextBox()
        //        {
        //            Text = "Managing Partner",
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            BorderThickness = new Thickness(0, 0, 0, 1),
        //            BorderBrush = Brushes.Black,
        //            IsReadOnly = true,
        //            Background = new SolidColorBrush() { Opacity = 0.3, Color = Colors.LightGray }
        //        });

        //        UniformGrid managPartGrid = new UniformGrid() { Margin = new Thickness(0, 0, 0, 20) };
        //        managPartGrid.Columns = 3;

        //        //StackPanel managPartGrid = new StackPanel();
        //        //managPartGrid.Orientation = Orientation.Horizontal;
        //        //managPartGrid.HorizontalAlignment = HorizontalAlignment.Center;

        //        managPartGrid.Children.Add(new Label { Content = "Managing Partner" });
        //        managPartGrid.Children.Add(new Label { Content = "Проценты" });
        //        managPartGrid.Children.Add(new Label { Content = "К выплате" });

        //        //newProjectPanel.Children.Add(managPartGrid);

        //        ComboBox managPartBox = new ComboBox();
        //        //managPartBox.VerticalAlignment = VerticalAlignment.Center;
        //        managPartBox.ItemsSource = AllPartners;
        //        managPartBox.DisplayMemberPath = "Name";
        //        managPartBox.SetBinding(ComboBox.SelectedValueProperty, new Binding() { Source = project, Path = new PropertyPath("ManagingPartner"), Mode = BindingMode.TwoWay });

        //        //StackPanel managPartGrid = new StackPanel();
        //        //managPartGrid.Orientation = Orientation.Horizontal;
        //        //managPartGrid.HorizontalAlignment = HorizontalAlignment.Center;

        //        managPartGrid.Children.Add(managPartBox);
        //        TextBox managPartPercent = new TextBox() { Text = project.isSuccess ? "40" : "25", MaxLength = 2, IsReadOnly = true };
        //        managPartGrid.Children.Add(managPartPercent);
        //        //managPartContent.Children.Add(new TextBox { Text = $"{project.MoneyDollar * int.Parse(managPartPercent.Text) / 100} $", IsReadOnly = true });

        //        ComboBox managPartMoneyBox = new ComboBox() { SelectedIndex = 0 };
        //        managPartMoneyBox.ItemsSource = new ObservableCollection<string>()
        //        {
        //            $"{project.MoneyDollar * int.Parse(managPartPercent.Text) / 100} $",
        //            $"{project.MoneyRouble * int.Parse(managPartPercent.Text) / 100} ₽",
        //            $"{project.MoneyEuro * int.Parse(managPartPercent.Text) / 100} €",
        //            $"{project.MoneyDollarCashless * int.Parse(managPartPercent.Text) / 100} $ б/н",
        //            $"{project.MoneyRoubleCashless * int.Parse(managPartPercent.Text) / 100} ₽ б/н",
        //            $"{project.MoneyEuroCashless * int.Parse(managPartPercent.Text) / 100} € б/н"
        //        };

        //        //StackPanel managPartMoneyPanel = new StackPanel();

        //        //managPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyDollar * int.Parse(managPartPercent.Text) / 100} $", IsReadOnly = true });
        //        //managPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyRouble * int.Parse(managPartPercent.Text) / 100} Р", IsReadOnly = true });
        //        //managPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyDollarCashless * int.Parse(managPartPercent.Text) / 100} $ б/н", IsReadOnly = true });
        //        //managPartMoneyPanel.Children.Add(new TextBox { Text = $"{project.MoneyRoubleCashless * int.Parse(managPartPercent.Text) / 100} Р б/н", IsReadOnly = true });

        //        managPartGrid.Children.Add(managPartMoneyBox);

        //        foreach (FrameworkElement el in managPartGrid.Children)
        //        {
        //            el.Margin = new Thickness(5);
        //        }

        //        newProjectPanel.Children.Add(managPartGrid);

        //        //Создание и заполнение полей для юристов
        //        newProjectPanel.Children.Add(new TextBox()
        //        {
        //            Text = "Юристы",
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            BorderThickness = new Thickness(0, 0, 0, 1),
        //            BorderBrush = Brushes.Black,
        //            IsReadOnly = true,
        //            Background = new SolidColorBrush() { Opacity = 0.3, Color = Colors.LightGray }
        //        });

        //        UniformGrid lawyersGrid = new UniformGrid() { Margin = new Thickness(0, 0, 0, 5) };
        //        lawyersGrid.Columns = 3;
        //        //lawyersGrid.Orientation = Orientation.Horizontal;
        //        //lawyersGrid.HorizontalAlignment = HorizontalAlignment.Center;

        //        lawyersGrid.Children.Add(new Label { Content = "Юристы" });
        //        lawyersGrid.Children.Add(new Label { Content = "Проценты" });
        //        lawyersGrid.Children.Add(new Label { Content = "К выплате" });

        //        //newProjectPanel.Children.Add(lawyersGrid);

        //        //ItemsControl LawyersControl = new ItemsControl();
        //        //LawyersControl.ItemsSource = project.Lawyers;
        //        //DataTemplate LawyersTemplate = new DataTemplate();
        //        //LawyersControl.ItemTemplate = LawyersTemplate;

        //        for(int i = 0; i < AllLawyers.Count; i++)
        //        {
        //            //StackPanel lawyersGrid = new StackPanel();
        //            //lawyersGrid.Orientation = Orientation.Horizontal;
        //            //lawyersGrid.HorizontalAlignment = HorizontalAlignment.Center;

        //            ComboBox lawyersBox = new ComboBox();
        //            lawyersBox.IsEditable = true;
        //            //lawyersBox.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = project.Lawyers[i], Mode = BindingMode.OneWay });
        //            lawyersBox.ItemsSource = AllLawyers;
        //            lawyersBox.DisplayMemberPath = "Name";
        //            //lawyersBox.SelectedItem = ;
        //            lawyersGrid.Children.Add(lawyersBox);

        //            TextBox lawyerPercent = new TextBox() { MaxLength = 2 };
        //            //lawyerPercent.SetBinding(TextBox.TextProperty, new Binding { Source = project.Lawyers[i].Projects[project.name], Path = new PropertyPath("Success"), Mode = BindingMode.TwoWay });
        //            lawyerPercent.MinWidth = 20;
        //            lawyerPercent.PreviewTextInput += NumberValidationTextBox;
        //            lawyersGrid.Children.Add(lawyerPercent);

        //            LawyerConverter converter = new LawyerConverter();
        //            TextBox lawyerMoney = new TextBox() { IsReadOnly = true };
        //            //lawyerMoney.SetBinding(TextBox.TextProperty, new Binding() { Source = lawyerPercent, Path = new PropertyPath("Text"), Mode = BindingMode.OneWay, Converter = converter, ConverterParameter = project.MoneyDollar });
        //            lawyersGrid.Children.Add(lawyerMoney);

        //            //newProjectPanel.Children.Add(lawyersGrid);
        //            //lawyersBox.SetBinding(ComboBox.SelectedValueProperty, new Binding() { Source = project, Path = new PropertyPath("ManagingPartner"), Mode = BindingMode.TwoWay });
        //            foreach (FrameworkElement el in lawyersGrid.Children)
        //            {
        //                el.Margin = new Thickness(5);
        //            }
        //        }

        //        newProjectPanel.Children.Add(lawyersGrid);

        //        Button AddLawyer = new Button() { Content = "Добавить юриста", HorizontalAlignment = HorizontalAlignment.Center };
        //        AddLawyer.Margin = new Thickness(0,0,0,20);
        //        //AddLawyer.Click += AddLawyer_Click;

        //        newProjectPanel.Children.Add(AddLawyer);

        //        //Создание и заполнение списка платежей
        //        newProjectPanel.Children.Add(new TextBox()
        //        {
        //            Text = "Платежи",
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            BorderThickness = new Thickness(0, 0, 0, 1),
        //            BorderBrush = Brushes.Black,
        //            IsReadOnly = true,
        //            Background = new SolidColorBrush() { Opacity = 0.3, Color = Colors.LightGray }
        //        });

        //        UniformGrid paymentsGrid = new UniformGrid() { Margin = new Thickness(0, 0, 0, 20) };
        //        paymentsGrid.Columns = 3;

        //        //StackPanel paymentsPanel = new StackPanel();
        //        //paymentsPanel.Orientation = Orientation.Horizontal;
        //        //paymentsPanel.HorizontalAlignment = HorizontalAlignment.Center;

        //        paymentsGrid.Children.Add(new Label { Content = "Сумма" });
        //        paymentsGrid.Children.Add(new Label { Content = "Дата" });
        //        paymentsGrid.Children.Add(new Label { Content = "Валюта" });

        //        //foreach (FrameworkElement el in paymentsPanel.Children) el.Margin = new Thickness(25, 0, 25, 0);

        //        foreach(Payment payment in project.Payments)
        //        {
        //            //StackPanel paymentsContent = new StackPanel();
        //            //paymentsContent.Orientation = Orientation.Horizontal;
        //            //paymentsContent.HorizontalAlignment = HorizontalAlignment.Center;

        //            TextBox paymentAmount = new TextBox() { };
        //            paymentAmount.SetBinding(TextBox.TextProperty, new Binding() { Source = payment, Path = new PropertyPath("Amount"), Mode = BindingMode.TwoWay, StringFormat = "N2" });
        //            paymentsGrid.Children.Add(paymentAmount);
        //            //paymentAmount.TextChanged += delegate(object s, TextChangedEventArgs e) { project.CountMoney(); };

        //            TextBox paymentDate = new TextBox();
        //            paymentDate.SetBinding(TextBox.TextProperty, new Binding() { Source = payment, Path = new PropertyPath("DateString") });
        //            paymentsGrid.Children.Add(paymentDate);

        //            TextBox paymentCurrency = new TextBox();
        //            paymentCurrency.SetBinding(TextBox.TextProperty, new Binding() { Source = payment, Path = new PropertyPath("Currency") });
        //            paymentsGrid.Children.Add(paymentCurrency);

        //            //newProjectPanel.Children.Add(paymentsContent);

        //            foreach (FrameworkElement el in paymentsGrid.Children)
        //            {
        //                el.Margin = new Thickness(5);
        //            }
        //        }

        //        newProjectPanel.Children.Add(paymentsGrid);

        //        /*Это вариант с TreeView
        //        TreeViewItem newProject = new TreeViewItem { Header = project.name };
        //        ProjectsTree.Items.Add(newProject);
        //        newProject.Items.Add(new TextBox { Text = "Партнёр" });
        //        TextBox origPart = new TextBox();
        //        ComboBox origPartBox = new ComboBox();
        //        origPartBox.ItemsSource = AllPartners;
        //        origPartBox.DisplayMemberPath = "Name";
        //        origPartBox.SetBinding(ComboBox.SelectedValueProperty, new Binding() { Source = project, Path = new PropertyPath("OriginatingPartner"), Mode = BindingMode.TwoWay });
        //        newProject.Items.Add(origPartBox);
        //        origPart.SetBinding(TextBox.TextProperty, new Binding() { Source = project, Path = new PropertyPath("OriginatingPartner.Name"), Mode = BindingMode.TwoWay });
        //        newProject.Items.Add(origPart);*/
        //    }
        //}


        //private void AddLawyer_Click(object sender, RoutedEventArgs e)
        //{
        //    ComboBox lawyersBox = new ComboBox();
        //    lawyersBox.IsEditable = true;
        //    //lawyersBox.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = project.Lawyers[i], Mode = BindingMode.OneWay });
        //    lawyersBox.ItemsSource = AllLawyers;
        //    lawyersBox.DisplayMemberPath = "Name";
        //    //lawyersBox.SelectedItem = ;
        //    lawyersGrid.Children.Add(lawyersBox);

        //    TextBox lawyerPercent = new TextBox() { MaxLength = 2 };
        //    //lawyerPercent.SetBinding(TextBox.TextProperty, new Binding { Source = project.Lawyers[i].Projects[project.name], Path = new PropertyPath("Success"), Mode = BindingMode.TwoWay });
        //    lawyerPercent.MinWidth = 20;
        //    lawyerPercent.PreviewTextInput += NumberValidationTextBox;
        //    lawyersGrid.Children.Add(lawyerPercent);

        //    LawyerConverter converter = new LawyerConverter();
        //    TextBox lawyerMoney = new TextBox() { IsReadOnly = true };
        //    //lawyerMoney.SetBinding(TextBox.TextProperty, new Binding() { Source = lawyerPercent, Path = new PropertyPath("Text"), Mode = BindingMode.OneWay, Converter = converter, ConverterParameter = project.MoneyDollar });
        //    lawyersGrid.Children.Add(lawyerMoney);
        //}

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ProjectsControl.Items.Count; i++)
            {
                Expander exp = VisualTreeHelper.GetChild(ProjectsControl.ItemContainerGenerator.ContainerFromIndex(i), 0) as Expander;
                if(!(sender as Expander).Equals(exp)) exp.IsExpanded = false;
            }
        }

        #region Методы кликов по кнопкам

        #region Проекты
        private void UploadFromSpreadsheetButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).GetInfoFromSpreadsheet(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_с актами.xlsx", 0, 1, 2, true);
            (DataContext as LCViewModel).GetInfoFromSpreadsheet(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_2_квартал.xlsx", 1, 3, 4, false);
        }

        private void makePaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            Project senderProject = (sender as Button).DataContext as Project;

            senderProject.PayMoney();
            (DataContext as LCViewModel).db.Update(senderProject);
            foreach(Lawyer lawyer in senderProject.Lawyers)
            {
                (DataContext as LCViewModel).db.Update(lawyer);
            }
            if (senderProject.ManagingPartner != null) (DataContext as LCViewModel).db.Update(senderProject.ManagingPartner);
            if (senderProject.OriginatingPartner != null) (DataContext as LCViewModel).db.Update(senderProject.OriginatingPartner);
        }

        private void MakeAllPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Project project in (DataContext as LCViewModel).AllProjects)
            {
                project.PayMoney();
                (DataContext as LCViewModel).db.Update(project);
            }
        }
        private void AddLawyerToProject_Click(object sender, RoutedEventArgs e)
        {
            Lawyer senderLawyer = (sender as Button).Tag as Lawyer;
            Project senderProject = (sender as Button).DataContext as Project;

            if (senderLawyer == null) return;

            bool doNotAddLawyer = false;
            foreach (Lawyer lawyer in senderProject.Lawyers) if (lawyer.Name == senderLawyer.Name) doNotAddLawyer = true;
            if (!doNotAddLawyer)
            {
                (DataContext as LCViewModel).db.Add(senderProject.Lawyers, senderLawyer);
                (DataContext as LCViewModel).db.Add(senderLawyer.LawyersProjects, new LawyersProject() { Name = senderProject.Name });
            }
            else MessageBox.Show("Этот юрист уже участвует в данном проекте");
        }

        private void AddOriginatorToProject_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as Project).OriginatingPartner = ((sender as Button).Tag as Partner);
            ((sender as Button).DataContext as Project).AddPartner((sender as Button).Tag as Partner);
            (sender as Button).Visibility = Visibility.Collapsed;
            ((sender as Button).DataContext as Project).OriginatorVisibilityTrigger = Visibility.Visible;
        }

        private void AddManagerToProject_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as Project).ManagingPartner = ((sender as Button).Tag as Partner);
            ((sender as Button).DataContext as Project).AddPartner((sender as Button).Tag as Partner);
            (sender as Button).Visibility = Visibility.Collapsed;
            ((sender as Button).DataContext as Project).ManagerVisibilityTrigger = Visibility.Visible;
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).db.Update((sender as Button).DataContext as Project);
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).db.Remove((DataContext as LCViewModel).AllProjects, (sender as Button).DataContext as Project);
        }
        #endregion

        #region Юристы
        private void AddLawyer_Click(object sender, RoutedEventArgs e)
        {
            Lawyer newLawyer = new Lawyer("Без имени", 0);
            (DataContext as LCViewModel).db.Add((DataContext as LCViewModel).AllLawyers, newLawyer);
        }

        private void SaveLawyer_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).db.Update((sender as Button).DataContext as Lawyer);
        }

        private void DeleteLawyer_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).db.Remove((DataContext as LCViewModel).AllLawyers, (sender as Button).DataContext as Lawyer);
        }
        #endregion

        #region Партнёры
        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            Partner newPartner = new Partner("Без имени");
            (DataContext as LCViewModel).db.Add((DataContext as LCViewModel).AllPartners, newPartner);
        }

        private void SavePartner_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).db.Update((sender as Button).DataContext as Partner);
        }

        private void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).db.Remove((DataContext as LCViewModel).AllPartners, (sender as Button).DataContext as Partner);
        }
        #endregion

        #endregion
    }
}
