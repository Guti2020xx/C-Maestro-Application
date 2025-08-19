using System.Data.SqlClient;
using System.Data;
using System;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Database_Control
{
    public partial class MainForm : Form
    {
        public SQL Connection { get; internal set; }
        public string FileName = "DataBaseOptions.options";
        public string[] Servers = new string[] { "GameStation\\SQLEXPRESS", "DESKTOP-E\\SQLEXPRESS", "LAPTOP-JP2PAISQ", "Cayden\\SQLEXPRESS01" };
        private byte[] DefaultPicture;

        private void SetCallbacks() // setting up UI components
        {
            CreateNewUser.Click += CreateNewUser_Click;
            LoginBtn.Click += LoginBtn_Click;
            ItemSearch.TextChanged += ItemSearch_TextChanged;
            SaveProduct.Click += SaveProduct_Click;
            ProductBack.Click += ProductBack_Click;
            SupplierSearch.TextChanged += SupplierSearch_TextChanged;
            referenceSearch.TextChanged += referenceSearch_TextChanged;
            DeleteRef.Click += button1_Click;
            PermissionsSearch.TextChanged += PermissionsSearch_TextChanged;
            PresetsSearch.TextChanged += PresetsSearch_TextChanged;
            CompanySearch.TextChanged += CompanySearch_TextChanged;
            CancelOrder.Click += CancelOrder_Click;
            SaveOrder.Click += SaveOrder_Click;
            ProductSearch.TextChanged += ProductSearch_TextChanged;
            LogoutBtn.Click += LogoutBtn_Click_1;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (Connection != null)
                Connection.Close();
        }

        public MainForm()
        {
            //declaring new SQL database connection 
            Connection = new SQL();
            string DataBase = "";
            bool Quit = false;
            Exception? E = null;

            do
            {
                if (!File.Exists(Application.StartupPath + FileName))
                {
                    File.WriteAllText(Application.StartupPath + FileName, String.Join("\n", Servers));
                }
                else
                { // loads server name data into a list of strings
                    List<string> Contents = new List<string>(File.ReadAllLines(Application.StartupPath + FileName));
                    List<string> Add = new List<string>();
                    foreach (var item in Servers)
                    {
                        if (!Contents.Contains(item))
                        {
                            Add.Add(item);
                        }
                    }
                    File.AppendAllText(Application.StartupPath + FileName, "\n" + String.Join("\n", Add));
                }

                if (E != null)
                {
                    MessageBox.Show(E.Message);
                }

                // initializes server list
                CreateServerDialog((string Text) => { DataBase = Text; }, () => { Quit = true; }, Application.StartupPath + FileName);
            } while (!Connection.Connect(DataBase, "root", "root", out E) && !Quit);

            InitializeComponent();
            SetCallbacks(); // ask Ian again

            //decoding string into an image
            DefaultPicture = ImageStringEncoderDecoder.ImageBytes(ProductImage.BackgroundImage);

            if (!Quit) // after logging in, meaning that user hasn't quit
            {
                // check if admin exists
                if (Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Position='Admin'", null), "Name").Count == 0)
                {
                    // it creates default admin if the admin account doesn't exist
                    Connection.InsertData("[Maestro].[dbo].[EMPLOYEE]", ("Position", "Admin"), ("Name", "Admin"), ("Username", "Admin"), ("Password", "Admin"), ("Image", DefaultPicture), ("History", "[" + DateTime.UtcNow.Date.ToString("dd / MM / yyyy") + "]: User Created through default"));
                }

                LogoutBtn.Visible = false;

                if (AutoLogin)
                {
                    LoginBtn_Click(this, EventArgs.Empty);
                }
            }
            else
            {
                this.Close();
            }
        }

        // initializes server list
        public static void CreateServerDialog(InputField.InputEnd EndAction, Action QuitAction, string File)
        {
            InitForm ServerForm = new InitForm(EndAction, QuitAction, File);

            ServerForm.ShowDialog();
        }

        private WindowData DisplayControl;
        private StatusType Stat;

        /* this class allows the window profile to be switched between tabs to make screenchanges
         * basically allowing us to switch the window index for all tabs and to present to the user
         * 
         */
        public class WindowProfile
        {
            private int Window;
            public int Tab { internal set; get; }

            public WindowProfile(int Window, int Tab)
            {
                this.Window = Window;
                this.Tab = Tab;
            }

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                return (obj as WindowProfile)?.Window == this.Window;
            }

            public override int GetHashCode() // referenced outside to get form/tab index
            {
                return this.Window;
            }
        }
        public class WindowType
        { // declaring window types with specific indexes for ease of access
            public static readonly WindowProfile Login = new WindowProfile(0, 0);
            public static readonly WindowProfile Delivery = new WindowProfile(1, 1);
            public static readonly WindowProfile Product = new WindowProfile(2, 2);
            public static readonly WindowProfile Company = new WindowProfile(3, 2);
            public static readonly WindowProfile Employee = new WindowProfile(4, 2);
            public static readonly WindowProfile Ordering = new WindowProfile(5, 4);
        }
        //public enum WindowType { Login, Delivery, Product, Company, Ordering }


        public void SetWindow(WindowProfile Type, Dictionary<string, object> DataIn)
        { //  class that allows to set window to a different one
            if (DisplayControl != null)
            {
                if (DisplayControl.OpenWindow(Type, DataIn))
                {
                    MainDisplay.SelectTab(Type.Tab);
                }
            }
            else
            { // set back to login home page
                MainDisplay.SelectTab(0);
            }
        }

        public enum List { OrderList, ListDisplay, OrderDisplay_Company, OrderDisplay_Product, UIList, ControlList, ProductItemList, ProductSupplier, ProductReferences, EmployeePermissions, EmployeePresets }
        public FlowLayoutPanel GetList(List Item)
        {
            switch (Item)
            { // to get lists which contain data for certain tabs and display it to the user
                case List.OrderList:
                    return OrderList;
                case List.ListDisplay:
                    return OptionsList;
                case List.OrderDisplay_Company:
                    return CompanyList;
                case List.OrderDisplay_Product:
                    return ProductList;
                case List.UIList:
                    return TopUI;
                case List.ControlList:
                    return ItemOptionPanel;
                case List.ProductItemList:
                    return ProductOptionList;
                case List.ProductSupplier:
                    return SupplierList;
                case List.ProductReferences:
                    return ReferencedOrdersList;
                case List.EmployeePermissions:
                    return PermissionsList;
                case List.EmployeePresets:
                    return PresetsList;
                default:
                    return OrderList;
            }
        }

        public enum PanelDetail { None, Delivery, Product, Company, Employee }
        public void SetDetailPanel(PanelDetail Detail)
        {
            DetailTabs.SelectTab((int)Detail);
        }

        private bool AutoLogin = false;

        // whenever login button is clicked
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            // getting information from employee entity and checking
            List<Dictionary<string, object>> User = Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Username=@User AND Password=@Pass", new (string, string)[] { ("@User", AutoLogin ? "Admin" : UserName.Text), ("@Pass", AutoLogin ? "Admin" : PassWord.Text) }), "Salesman_ID", "Name", "Position", "Password");
            if (User.Count == 0)
            {  // if there is no entry found then employee doesn't exist and login is invalid
                MessageBox.Show("Login Invalid");
            }
            else
            {   // grabs information from the table on user access rights and moves into the homepage
                Stat = new StatusType(StatusType.CreateFrom((string)User[0]["Position"]), (int)User[0]["Salesman_ID"], (string)User[0]["Name"], (string)User[0]["Position"], (string)User[0]["Password"]);
                Connection.CreateChangeCallback("UserCheck", OnUserChange, this, null, "[dbo].[EMPLOYEE]", ("Salesman_ID=@ID", new (string, string)[] { ("@ID", User[0]["Salesman_ID"].ToString()) }), "Position", "History");
                DisplayControl = new WindowData(this, Stat);
                LogoutBtn.Visible = true;
                SetWindow(WindowType.Delivery, new Dictionary<string, object>() { { "INIT", (string)User[0]["Position"] }, { "NAME", (string)User[0]["Name"] } });
            }
        }

        private void LogoutBtn_Click_1(object sender, EventArgs e)
        {
            LogOut(); // calls to logout user
        }

        public void LogOut() // returns user to login form
        {
            LogoutBtn.Visible = false;
            UserName.Text = "";
            PassWord.Text = "";
            SetWindow(WindowType.Login, null);
            Connection.ResetCallback();
        }

        // adding new employee to the database
        private void CreateNewUser_Click(object sender, EventArgs e)
        {
            // data validation check, making sure there is a value for username and password can't be less than 10 characters
            if (!string.IsNullOrEmpty(UserName.Text) && PassWord.Text.Length >= 10)
            {   // checking if entity already has an employee with that username
                List<Dictionary<string, object>> User = Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Username=@User", new (string, string)[] { ("@User", UserName.Text) }), "Salesman_ID");
                if (User.Count > 0)
                {   // if it does then display message
                    MessageBox.Show("Username taken, please pick another");
                }
                else
                {   // if it is a new user then 
                    string Name = Interaction.InputBox("Enter Employee Name");
                    if (!string.IsNullOrEmpty(Name))
                    {   // insert data into the database, and assigning default status to grunt
                        Connection.InsertData("[Maestro].[dbo].[EMPLOYEE]", ("Position", "Grunt"), ("Image", DefaultPicture), ("Name", Name), ("Username", UserName.Text), ("Password", PassWord.Text), ("History", "[" + DateTime.UtcNow.Date.ToString("dd / MM / yyyy") + "]: User Created through login"));
                        // getting data from the database 
                        User = Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Username=@User AND Password=@Pass", new (string, string)[] { ("@User", UserName.Text), ("@Pass", PassWord.Text) }), "Salesman_ID", "Name", "Position", "Password");

                        // gets permission object. !!!!
                        Stat = new StatusType(StatusType.CreateFrom((string)User[0]["Position"]), (int)User[0]["Salesman_ID"], (string)User[0]["Name"], (string)User[0]["Position"], (string)User[0]["Password"]);
                        DisplayControl = new WindowData(this, Stat);
                        Connection.CreateChangeCallback("UserCheck", OnUserChange, this, null, "[dbo].[EMPLOYEE]", ("Salesman_ID=@ID", new (string, string)[] { ("@ID", User[0]["Salesman_ID"].ToString()) }), "Position", "History");
                        LogoutBtn.Visible = true;
                        // afterwards then sends user back to delivery homepage
                        SetWindow(WindowType.Delivery, new Dictionary<string, object>() { { "INIT", (string)User[0]["Position"] }, { "NAME", (string)User[0]["Name"] } });
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid name");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter " + (string.IsNullOrEmpty(UserName.Text) ? ("a valid Username " + (PassWord.Text.Length < 10 ? "and " : "")) : "") + (PassWord.Text.Length < 10 ? "a Password with 10 or more characters" : ""));
            }
        }

        private void ItemSearch_TextChanged(object sender, EventArgs e)
        { // when searching for an item within a tab it will sort the item to the top, if not found then
            // alphabetical closest will be at the top. Same goes for the following 4 functions 
            // that detect a change in the search box
            WindowData.SortItems(GetList(List.OrderList), ItemSearch.Text);
        }

        private void CompanySearch_TextChanged(object sender, EventArgs e)
        {
            WindowData.SortItems(GetList(List.OrderDisplay_Company), CompanySearch.Text);
        }

        private void ProductSearch_TextChanged(object sender, EventArgs e)
        {
            WindowData.SortItems(GetList(List.OrderDisplay_Product), ProductSearch.Text);
        }

        private void SupplierSearch_TextChanged(object sender, EventArgs e)
        {
            WindowData.SortItems(GetList(List.ProductSupplier), SupplierSearch.Text);
        }

        private void referenceSearch_TextChanged(object sender, EventArgs e)
        {
            WindowData.SortItems(GetList(List.ProductReferences), referenceSearch.Text);
        }

        //on clicking on cancel order returns the user to delivery tab
        private void CancelOrder_Click(object sender, EventArgs e)
        {
            SetWindow(WindowType.Delivery, new Dictionary<string, object>() { { "INIT", "" } });
        }

        public Action SaveOrderAction;

        // saving order to database on click
        private void SaveOrder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(OrderPassword.Text))
            {
                if (OrderPassword.Text.Equals(Stat.GetPass()))
                {
                    if (SaveOrderAction != null)
                        SaveOrderAction(); // calling function to save data into the database
                }
                else
                {
                    MessageBox.Show("Enter correct password to complete");
                }
            }
            else
            {
                MessageBox.Show("Enter password to complete");
            }
        }

        public void resetPass()
        {
            OrderPassword.Text = "";
            ProductPassword.Text = "";
        }

        public string GetMemo()
        {
            return Memo.Text;
        }

        public void SetMemo(string MemoString)
        {
            Memo.Text = MemoString;
        }

        public void SetReferencedNum(string Num)
        {
            ReservedLable.Text = Num;
        }

        public void SetPrice(string Price)
        {
            ProductPrice.Text = Price;
        }

        public float GetProductPrice()
        {
            if (float.TryParse(ProductPrice.Text, out float Result))
            {
                return Result;
            }
            return -1;
        }

        public string GetProductDesc()
        {
            return ProductDescript.Text;
        }

        public void SetProductDesc(string MemoString, bool ReadOnly)
        {
            ProductDescript.ReadOnly = ReadOnly;
            ProductDescript.Text = MemoString;
        }

        public int GetPrepTime()
        {
            if (int.TryParse(PrepTime.Text, out int Result))
            {
                return Math.Max(Result, 0);
            }
            return 0;
        }

        public void SetPrepTime(string Time)
        {
            PrepTime.Text = Time;
        }

        // button to return to Delivery tab
        private void ProductBack_Click(object sender, EventArgs e)
        {
            SetWindow(WindowType.Delivery, new Dictionary<string, object>() { { "INIT", "" } }); // sending null data to a delivery page reinitializes the window
        }

        public Action SaveProductAction;

        // when product save button is clicked
        private void SaveProduct_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProductPassword.Text))
            {
                if (ProductPassword.Text.Equals(Stat.GetPass()))
                {
                    if (SaveProductAction != null)
                        SaveProductAction(); // calls function to save product into database
                }
                else
                {
                    MessageBox.Show("Enter correct password to complete");
                }
            }
            else
            {
                MessageBox.Show("Enter password to complete");
            }
        }

        public void SetTotalAmount(int Amount)
        {
            ProductAmount.Text = Amount.ToString();
        }

        public int GetTotalAmount()
        {
            if (int.TryParse(ProductAmount.Text, out int Result))
            {
                return Result;
            }
            return -1;
        }

        public void SetProductName(string Name, bool ReadOnly)
        {
            ProductName.ReadOnly = ReadOnly;
            ProductName.Text = Name;
        }

        public string GetProductName()
        {
            return ProductName.Text;
        }

        public void SetEmployeePass(string Name, bool PasswordChar, bool ReadOnly)
        {
            EmployeePass.ReadOnly = ReadOnly;
            EmployeePass.Enabled = !ReadOnly;
            EmployeePass.PasswordChar = PasswordChar ? '*' : '\0';
            EmployeePass.Text = Name;
        }

        public string GetEmployeePass()
        {
            return EmployeePass.Text;
        }

        public void SetEmployeeUser(string Name, bool ReadOnly)
        {
            EmployeeUser.ReadOnly = ReadOnly;
            EmployeeUser.Enabled = !ReadOnly;
            EmployeeUser.Text = Name;
        }

        public string GetEmployeeUser()
        {
            return EmployeeUser.Text;
        }

        // deletes orders associated with a company when a corrext password is entered
        private void button1_Click(object sender, EventArgs e)
        {//  deleting products from deliveries
            WindowData.CreateDeleteDialog("Enter Password To Delete Links", (string Text) =>
            {   // password check
                if (Text.Equals(Stat.GetPass()) && Stat.HasAbility(StatusType.Action.CanDeleteDelivery))
                {
                    List<(Control, WindowData.CollectionReturn Call)> Removes = WindowData.GetSelectedObjects("OrderProductSelect");
                    Dictionary<string, object> DataIn = null;
                    List<string> DeletedItemsProduct = new List<string>();
                    List<string> DeletedItemsOrder = new List<string>();

                    foreach (var item in Removes)
                    {
                        Dictionary<string, object> Data = item.Call();
                        string ProductID = Data["Product_ID"].ToString();
                        string OrderID = Data["Order_ID"].ToString();

                        //getting bundles and deliveries associated with each other
                        if (!DeletedItemsProduct.Contains(Data["ProductName"].ToString()))
                            DeletedItemsProduct.Add(Data["ProductName"].ToString());

                        if (!DeletedItemsOrder.Contains(OrderID))
                            DeletedItemsOrder.Add(OrderID);

                        DataIn = (Dictionary<string, object>)Data["INFO"];

                        List<Dictionary<string, object>> Delivery = Connection.GetData("[Maestro].[dbo].[DELIVERIES]", ("Order_ID=@ID", new (string, string)[] { ("@ID", OrderID) }), "Bundle_ID");
                        List<Dictionary<string, object>> Bundles = Connection.GetData("[Maestro].[dbo].[BUNDLES]", ("Bundle_ID=@ID", new (string, string)[] { ("@ID", Delivery[0]["Bundle_ID"].ToString()) }), "Bundle_ID", "Product_ID", "Quantity", "Delivered");

                        int UpForDelete = 0;
                        int Restock = 0;
                        for (int i = 0; i < Bundles.Count; i++) // loop to check which inventory must be restocked as delivery wasn't fulfilled
                        {
                            if (Bundles[i]["Product_ID"].ToString().Equals(ProductID))
                            {
                                UpForDelete++;
                                // adding stock from delete order back into inventory
                                Restock += ((int)Bundles[i]["Delivered"] == 0 ? (int)Bundles[i]["Quantity"] : 0);
                            }
                        }

                        // if count of items deleted is not equal to the count of bundles that were selected to be deleted then
                        if (UpForDelete != Bundles.Count)
                        { // !!!!!!!!
                            // deleting specific bundles from a delivery
                            Connection.DeleteData("[Maestro].[dbo].[BUNDLES]", ("Bundle_ID=@ID AND Product_ID=@PID", new (string, string)[] { ("@ID", Delivery[0]["Bundle_ID"].ToString()), ("@PID", ProductID) }));
                        }
                        else // if all are gone then deleting Orders from deliveries along with the associated Bundle
                        {
                            Connection.DeleteData("[Maestro].[dbo].[DELIVERIES]", ("Order_ID=@ID", new (string, string)[] { ("@ID", OrderID) }));
                            Connection.DeleteData("[Maestro].[dbo].[BUNDLES]", ("Bundle_ID=@ID AND Product_ID=@PID", new (string, string)[] { ("@ID", Delivery[0]["Bundle_ID"].ToString()), ("@PID", ProductID) }));
                        }

                        List<Dictionary<string, object>> ProductEntry = Connection.GetData("[Maestro].[dbo].[PRODUCTS]", ("Product_ID=@ID", new (string, string)[] { ("@ID", ProductID) }), "Available_Amt");
                        // updating the database with the new quantities. 
                        Connection.UpdateData("[Maestro].[dbo].[PRODUCTS]", ("Product_ID=@ID", new (string, string)[] { ("@ID", ProductID) }), ("Available_Amt", ((int)ProductEntry[0]["Available_Amt"] + Restock).ToString()));
                    }
                    // update access log
                    WindowData.UpdateUserHistory(this, Stat.GetIDNumber(), "User deleted: " + String.Join(", ", DeletedItemsProduct) + "\n\tFrom Orders:\n\t|[" + String.Join(", ", DeletedItemsOrder) + "]");
                    MessageBox.Show("Deletion Complete");
                    // sending user to product window
                    SetWindow(MainForm.WindowType.Product, DataIn);
                }
                else
                { // checking if user has the access to delete a delivery
                    if (Stat.HasAbility(StatusType.Action.CanDeleteDelivery))
                        MessageBox.Show("User does not have access to this ability");
                    else
                        MessageBox.Show("Incorrect Password");
                }
            });
        }

        public void SetPropertiesWindow(int Index)
        {
            ProductProperties.SelectTab(Index);
        }

        public void SetPhone(string Phone)
        {
            CompanyPhone.Text = Phone;
        }

        public void SetEmail(string Email)
        {
            CompanyEmail.Text = Email;
        }

        public string GetPhone()
        {
            return CompanyPhone.Text;
        }

        private bool IsValidEmail(string eMail)
        { // simple way that Ian came up with to validate email
            bool Result = false;

            try
            { // checking if valid email by seeing if the @ comes before the . 
                var eMailValidator = new System.Net.Mail.MailAddress(eMail);

                Result = (eMail.LastIndexOf(".") > eMail.LastIndexOf("@"));
            }
            catch
            {
                Result = false;
            };

            return Result;
        }

        public (bool, string) GetEmail()
        {
            if (string.IsNullOrEmpty(CompanyEmail.Text))
                return (true, "");
            if (IsValidEmail(CompanyEmail.Text))
            {
                return (true, CompanyEmail.Text);
            }
            return (false, "");
        }

        public string GetAddress()
        {
            return CompanyAddress.Text;
        }

        public void FillDeliveryDisplay(Dictionary<string, object> ListItems) // shows user information of the delivery selected
        {  // filling deliveries display whenever an item within a delivery is selected
            DeliveryInfo.Clear();
            DeliveryInfo.AppendText("Order ID: " + ListItems["Order_ID"].ToString() +
                "\n--------------------------------------\n" +
                "Order Status: " + WindowData.GetOrderStatus((int)ListItems["Status"]) +
                "\n--------------------------------------\n" +
                "Order creation date: " + ListItems["CreationDate"].ToString());

            // getting information from the employee table
            List<Dictionary<string, object>> Employee = Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Salesman_ID=@ID", new (string, string)[] { ("@ID", ListItems["Salesman_ID"].ToString()) }), "Name", "Username", "Position");
            if (Employee.Count > 0) // checks if there is an employee that made the order within the database, to see if employee still exists in table.
                // displaying employee which created order
                DeliveryInfo.AppendText("Order Created by: " + Employee[0]["Name"].ToString() + " | Current Position: " + Employee[0]["Position"].ToString() + "\n");
            else
                DeliveryInfo.AppendText("Order Created by: [EMPLOYEE NOT FOUND]\n");

            List<Dictionary<string, object>> Bundles = Connection.GetData("[Maestro].[dbo].[BUNDLES]", ("Bundle_ID=@ID", new (string, string)[] { ("@ID", ListItems["Bundle_ID"].ToString()) }), "Product_ID", "Delivered", "Quantity");
            List<Dictionary<string, object>> Products;
            List<Dictionary<string, object>> Company;

            // getting info to see which company the order will be delivered to and displaying it to user
            Company = Connection.GetData("[Maestro].[dbo].[COMPANIES]", ("Company_ID=@ID", new (string, string)[] { ("@ID", ListItems["Company_ID"].ToString()) }), "Name", "Address");
            DeliveryInfo.AppendText("[DELIVER TO]: " + Company[0]["Name"].ToString() + "\n[ADDRESS]: " + Company[0]["Address"].ToString() + "\n\n");
            DateTime CreationDate = DateTime.ParseExact(ListItems["CreationDate"].ToString(), "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DeliveryInfo.AppendText("-------------[ORDER CONENTS]-------------\n");
            DeliveryInfo.AppendText(string.Format("\t|{0,15}|{1,10}|{2,10}|{3,11}|{4,12}|\n", "Name(Quantity)", "Price($)", "Supplier", "[DELIVERED]", "[DUE DATE]"));

            foreach (var item in Bundles)
            {   // getting bundles within the delivery and the products to display to user
                Products = Connection.GetData("[Maestro].[dbo].[PRODUCTS]", ("Product_ID=@ID", new (string, string)[] { ("@ID", item["Product_ID"].ToString()) }), "Name", "Price", "Time", "Supplier");
                Company = Connection.GetData("[Maestro].[dbo].[COMPANIES]", ("Company_ID=@ID", new (string, string)[] { ("@ID", Products[0]["Supplier"].ToString()) }), "Name");
                DeliveryInfo.AppendText(string.Format("\t|{0,15}|{1,10}|{2,10}|{3,11}|{4,12}|\n", (Products[0]["Name"].ToString() + "(" + item["Quantity"].ToString() + ")"),
                    Products[0]["Price"].ToString(), Company[0]["Name"].ToString(), ((int)item["Delivered"] == 1 ? "[YES]" : "[NO]"), (CreationDate.AddDays((int)Products[0]["Time"]).ToString("dd/MM/yyyy"))));
            }

            // additional infor for access log or employee to view
            DeliveryInfo.AppendText("[MEMO]:\n");
            DeliveryInfo.AppendText(ListItems["Memo"].ToString() + "\n");
            DeliveryInfo.AppendText("[ORDER HISTORY]:\n");
            DeliveryInfo.AppendText(ListItems["History"].ToString());
        }

        public void FillProductDisplay(Dictionary<string, object> ListItems)
        {   // filling product display with selected product info
            ProductInfo.Clear();
            ProductInfo.AppendText("Product ID: " + ListItems["Product_ID"].ToString() + " | Name: " + ListItems["Name"].ToString() +
                "\n--------------------------------------\n" +
                "Available Amount: " + ListItems["Available_Amt"].ToString() +
                "\n--------------------------------------\n" +
                "Price: $" + ListItems["Price"].ToString() +
                "\n--------------------------------------\n" +
                "Prep Time: " + ListItems["Time"].ToString() + "\n");

            // getting data from database
            List<Dictionary<string, object>> Comp = Connection.GetData("[Maestro].[dbo].[COMPANIES]", ("Company_ID=@ID", new (string, string)[] { ("@ID", ListItems["Supplier"].ToString()) }), "Name", "Address");
            ProductInfo.AppendText("Supplier: " + Comp[0]["Name"].ToString() + "\n");
            ProductInfo.AppendText("Address: " + Comp[0]["Address"].ToString() + "\n\n");
            ProductInfo.AppendText("Description:\n" + ListItems["Description"].ToString());
            ProductInfo.AppendText("[PRODUCT HISTORY]:\n");
            ProductInfo.AppendText(ListItems["History"].ToString());

            List<Dictionary<string, object>> ImageGrab = Connection.GetData("[Maestro].[dbo].[PRODUCTS]", ("Product_ID=@ID", new (string, string)[] { ("@ID", ListItems["Product_ID"].ToString()) }), "Image");
            // displaying image
            SetImage((byte[])ImageGrab[0]["Image"], Picture.Detail_Product);
        }

        public void FillCompanyDisplay(Dictionary<string, object> ListItems)
        {   // filling company display with selected company info
            CompanyInfo.Clear();
            CompanyInfo.AppendText("Name: " + ListItems["Name"].ToString() + "\n");
            CompanyInfo.AppendText("\n[CONTACT INFO]:\n");
            CompanyInfo.AppendText("Phone: " + ListItems["Phone"].ToString() + "\n");
            CompanyInfo.AppendText("Email: " + ListItems["Email"].ToString() + "\n");
            CompanyInfo.AppendText("Address: " + ListItems["Address"].ToString() + "\n");
            CompanyInfo.AppendText("\nDescription:\n" + ListItems["Description"].ToString() + "\n");

            // getting data from database
            List<Dictionary<string, object>> ImageGrab = Connection.GetData("[Maestro].[dbo].[COMPANIES]", ("Company_ID=@ID", new (string, string)[] { ("@ID", ListItems["Company_ID"].ToString()) }), "Image");
            // displaying image
            SetImage((byte[])ImageGrab[0]["Image"], Picture.Detail_Company);
        }

        public void FillEmployeeDisplay(Dictionary<string, object> ListItems)
        { // same logic as other fill methods to display info about selected employee
            EmployeeInfo.Clear();
            EmployeeInfo.AppendText("Employee: " + ListItems["Name"].ToString() + "\n\n");
            EmployeeInfo.AppendText("Position: " + ListItems["Position"].ToString() + "\n");

            StatusType EmpStat = new StatusType(StatusType.CreateFrom((string)ListItems["Position"]), 0, "", "", "");
            List<string> Perms = EmpStat.PrintStats();
            EmployeeInfo.AppendText("Abilities: { ");
            foreach (var item in Perms)
            {
                EmployeeInfo.AppendText(item + " ");
            }
            EmployeeInfo.AppendText("}\n");

            EmployeeInfo.AppendText("[USERNAME]: " + ListItems["Username"].ToString() + "\n");

            EmployeeInfo.AppendText("-----------------[USER HISTORY]-----------------\n");
            EmployeeInfo.AppendText(ListItems["History"].ToString());

            List<Dictionary<string, object>> ImageGrab = Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Salesman_ID=@ID", new (string, string)[] { ("@ID", ListItems["Salesman_ID"].ToString()) }), "Image");
            SetImage((byte[])ImageGrab[0]["Image"], Picture.Detail_Employee);
        }

        private void PresetsSearch_TextChanged(object sender, EventArgs e)
        {   // when input is detected within the preset searchbox, desired preset is sorted to the top
            WindowData.SortItems(GetList(List.EmployeePresets), PresetsSearch.Text);
        }

        private void PermissionsSearch_TextChanged(object sender, EventArgs e)
        {   //when input is detected with the employee access permission, desired permissions is sorted to the top
            WindowData.SortItems(GetList(List.EmployeePermissions), PermissionsSearch.Text);
        }

        public void OpenImage() // function to load image into the window
        {
            OpenFileDialog FileOpen = new OpenFileDialog(); // to read from a file

            FileOpen.Filter = "Image Files | *.png";

            if (FileOpen.ShowDialog() == DialogResult.OK)
            {
                try // trying to load image from file
                {
                    ProductImage.BackgroundImage = Image.FromFile(FileOpen.FileName);

                }
                catch (Exception e)
                {   // if photo size is too large then display error message.
                    MessageBox.Show("Image file too large, please keep it under 50Kb");
                }
            }
        }

        public byte[] GetImage()
        {
            return ImageStringEncoderDecoder.ImageBytes(ProductImage.BackgroundImage);
        }

        public enum Picture { EditPage, Detail_Product, Detail_Company, Detail_Employee }

        // method to show image within a specific display
        public void SetImage(byte[] Data, Picture Pic)
        {
            byte[] PicData = Data.Length == 0 ? DefaultPicture : Data;

            switch (Pic)
            {
                case Picture.EditPage:
                    ProductImage.BackgroundImage = ImageStringEncoderDecoder.GetImage(PicData, DefaultPicture);
                    break;
                case Picture.Detail_Product:
                    ProductPic.BackgroundImage = ImageStringEncoderDecoder.GetImage(PicData, DefaultPicture);
                    break;
                case Picture.Detail_Company:
                    CompanyPic.BackgroundImage = ImageStringEncoderDecoder.GetImage(PicData, DefaultPicture);
                    break;
                case Picture.Detail_Employee:
                    EmployeePic.BackgroundImage = ImageStringEncoderDecoder.GetImage(PicData, DefaultPicture);
                    break;
                default:
                    break;
            }
        }

        void OnUserChange(MainForm Form, Dictionary<string, object> Data)
        {
            List<Dictionary<string, object>> Position = Connection.GetData("[Maestro].[dbo].[EMPLOYEE]", ("Salesman_ID=@ID", new (string, string)[] { ("@ID", Stat.GetIDNumber().ToString()) }), "Position");
            if (Position.Count == 0)
            {
                LogOut();
                MessageBox.Show("You have been deleted");
            }
            else
            {
                if (!string.IsNullOrEmpty(Position[0]["Position"].ToString()))
                {
                    if (!Position[0]["Position"].ToString().Equals(Stat.GetPosition()))
                    {
                        MessageBox.Show("Your permissions have been changed from: " + Stat.GetPosition() + " to: " + Position[0]["Position"].ToString() + "\n"
                            + "Before:\n{" + string.Join(' ', Stat.PrintStats()) + "}\nNow:\n{" +
                            string.Join(' ', StatusType.PrintStats(StatusType.CreateFrom(Position[0]["Position"].ToString()))) + "}");
                        LogOut();
                    }
                }
            }
        }
    }
}