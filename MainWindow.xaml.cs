using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OfficeOpenXml;

namespace LawCalculator_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Project> AllProjects { get; set; } = new ObservableCollection<Project>();
        private ObservableCollection<Lawyer> AllLawyers { get; set; } = new ObservableCollection<Lawyer>();
        private ObservableCollection<Partner> AllPartners { get; set; } = new ObservableCollection<Partner>();

        //private UniformGrid lawyersGrid;
        public MainWindow()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            InitializeComponent();
            DataContext = new LCViewModel();
            //this.DataContext = this;

            Partner Kovalev = new Partner("Сергей Ковалев");
            Partner Kislov = new Partner("Сергей Кислов");
            Partner Tugushi = new Partner("Дмитрий Тугуши");
            Project Clarke = new Project("Clarke Invest Group", Kislov, Kovalev, false);
            Lawyer Ivan = new Lawyer("Иван", 60000);
            Lawyer Roman = new Lawyer("Роман", 80000);

            //Clarke.AddLawyer(Ivan, 5);
            //Clarke.AddLawyer(Roman, 7);
            //Clarke.RemoveLawyer(Ivan);

            AllProjects.Add(Clarke);

            AllPartners.Add(Kovalev);
            AllPartners.Add(Kislov);
            AllPartners.Add(Tugushi);

            AllLawyers.Add(Ivan);
            AllLawyers.Add(Roman);

            SearchBox.TextChanged += SearchBox_TextChanged;
            ShowToPayBox.Checked += ShowToPayBox_Checked;
            ShowToPayBox.Unchecked += ShowToPayBox_Unchecked;

            AllProjects = new ObservableCollection<Project>(AllProjects.OrderBy(i => i));

            //LawyersPanel.ItemsSource = AllLawyers;
            //ProjectsControl.ItemsSource = AllProjects;
            //GetInfoFromSpreadsheet();
            //MakeProjects();
            //foreach (Project project in AllProjects) project.CountMoney();

        }

        private void ShowToPayBox_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ProjectsControl.Items.Count; i++)
            {
                ((UIElement)ProjectsControl.ItemContainerGenerator.ContainerFromIndex(i)).Visibility = Visibility.Visible;
            }

            //foreach (var proj in ProjectsPanel.Children)
            //{
            //    if (proj.GetType() == typeof(Expander))
            //    {
            //        (proj as Expander).Visibility = Visibility.Visible;
            //    }
            //}
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

            //foreach (var proj in ProjectsPanel.Children)
            //{
            //    if (proj.GetType() == typeof(ItemsControl))
            //    {
            //        bool hide = true;
            //        foreach (double money in ((proj as ItemsControl).DataContext as Project).Money) if (money > 0) hide = false;
            //        if (hide) (proj as ItemsControl).Visibility = Visibility.Collapsed;
            //    }
            //}
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

            //foreach (var proj in ProjectsPanel.Children)
            //{
            //    //MessageBox.Show(ProjectsPanel.Children.Count.ToString());
            //    if (proj.GetType() == typeof(Expander))
            //    {
            //        if (!(proj as Expander).Header.ToString().ToLower().Contains(SearchBox.Text.ToLower())) (proj as Expander).Visibility = Visibility.Collapsed;
            //        else (proj as Expander).Visibility = Visibility.Visible;
            //        if (SearchBox.Text == string.Empty) (proj as Expander).Visibility = Visibility.Visible;
            //    }
            //}
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
                //while(!Type.Equals(sender.GetType(), new ItemsControl()))
                //{ 
                //    sender = VisualTreeHelper.GetParent(sender as DependencyObject);
                //    MessageBox.Show(sender.ToString());
                //}

                Expander exp = VisualTreeHelper.GetChild(ProjectsControl.ItemContainerGenerator.ContainerFromIndex(i), 0) as Expander;
                if(!(sender as Expander).Equals(exp)) exp.IsExpanded = false;
            }
        }

        private void AddLawyer_Click(object sender, RoutedEventArgs e)
        {
            Lawyer newLawyer = new Lawyer("Без имени", 0);
            (DataContext as LCViewModel).AllLawyers.Add(newLawyer);
        }

        private void makePaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as Project).PayMoney();
        }

        private void AddLawyerToProject_Click(object sender, RoutedEventArgs e)
        {
            //Сделать выбор юриста в проект из коллекции юристов

            //Lawyer lawyer = new Lawyer(((sender as Button).Tag as Lawyer).Name, 0);
            bool doNotAddLawyer = false;
            foreach (Lawyer lawyer in ((sender as Button).DataContext as Project).Lawyers) if (lawyer.Name == ((sender as Button).Tag as Lawyer).Name) doNotAddLawyer = true;
            if (!doNotAddLawyer)
            {
                ((sender as Button).DataContext as Project).AddLawyer((sender as Button).Tag as Lawyer, 0);
                //LawyerContext.UpdateLawyer((sender as Button).Tag as Lawyer);
                //LawyerContext.UpdateProject((sender as Button).DataContext as Project);
            }
            else MessageBox.Show("Этот юрист уже участвует в данном проекте");
            
        }

        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).AllPartners.Add(new Partner("Без имени"));
        }

        private void AddOriginatorToProject_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as Project).AddPartner((sender as Button).Tag as Partner);
            (sender as Button).Visibility = Visibility.Collapsed;
            ((sender as Button).DataContext as Project).OriginatorVisibilityTrigger = Visibility.Visible;
        }

        private void AddManagerToProject_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Button).DataContext as Project).AddPartner((sender as Button).Tag as Partner);
            (sender as Button).Visibility = Visibility.Collapsed;
            ((sender as Button).DataContext as Project).ManagerVisibilityTrigger = Visibility.Visible;
        }

        private void UploadFromSpreadsheetButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LCViewModel).GetInfoFromSpreadsheet();
        }

        private void MakeAllPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Project project in (DataContext as LCViewModel).AllProjects) project.PayMoney();
        }

        //private void EditOriginator_Click(object sender, RoutedEventArgs e)
        //{
        //    ((sender as Button).DataContext as Project).RemovePartner((sender as Button).Tag as Partner);
        //    (sender as Button).Visibility = Visibility.Collapsed;
        //    (DataContext as LCViewModel).OriginatorVisibilityTrigger = Visibility.Visible;
        //}

        //private void GetInfoFromSpreadsheet()
        //{
        //    var sheetAdress = new FileInfo(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_.xlsx");
        //    using (var p = new ExcelPackage(sheetAdress))
        //    {
        //        int[] sheets = new int[] { 1, 3, 4 };
        //        foreach (int sheet in sheets)
        //        {
        //            var currentSheet = p.Workbook.Worksheets[sheet];
        //            for (int row = 4; row < 600; row++)
        //            {
        //                if (!string.IsNullOrEmpty((string)currentSheet.Cells[row, 4].Value))
        //                {
        //                    if (currentSheet.Cells[row, 4].Value.ToString() != "Итого за день:" && currentSheet.Cells[row, 4].Value.ToString() != "Исходящий остаток")
        //                    {
        //                        string projName = currentSheet.Cells[row, 4].Value.ToString().Trim();
        //                        bool doNotAddProj = false;
        //                        //MessageBox.Show(projName);
        //                        Payment pay = new Payment() { Amount = (double)currentSheet.Cells[row, 5].Value, Currency = sheet == 1 ? CurrencyType.Dollar : sheet == 3 ? CurrencyType.Euro : CurrencyType.Rouble , Date = DateTime.FromOADate((double)currentSheet.Cells[row, 2].Value) };
        //                        Project newProj = new Project(projName, AllPartners[new Random().Next(0, AllPartners.Count)],
        //                                AllPartners[new Random().Next(0, AllPartners.Count)], false);
        //                        foreach (Project proj in AllProjects)
        //                        {
        //                            if (proj.Name == projName)
        //                            {
        //                                doNotAddProj = true;
        //                                proj.Payments.Add(pay);
        //                            }
        //                        }

        //                        if (!doNotAddProj)
        //                        {
        //                            newProj.Payments.Add(pay);
        //                            AllProjects.Add(newProj);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
