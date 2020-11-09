/*                      Lab 3: Navigate the World
 * Najeebulla Hussaini
 * 100596841
 * 2020-11-08
 * Purpose:  keeping track of the owners of the current shares, the number of shares available, the price that a share
             was sold for (share prices fluctuate)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace NETD3202_F2020_LAB3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //if create entry is clicked 
        private void btnCreateEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Grabbing values from form
                string name = txtName.Text;
                string datePurchased = dtPicker.SelectedDate.ToString();
                string shareType = "";
                if (name != "")
                {
                    if (int.TryParse(txtShares.Text, out int shares))
                    {
                        if (datePurchased != "")
                        {
                            if (radShareTypeCommon.IsChecked == true)
                            {
                                shareType = "common";
                            }
                            else if (radShareTypePreferred.IsChecked == true)
                            {
                                shareType = "preferred";
                            }
                            //connectString for database
                            string connectString = Properties.Settings.Default.connect_string;
                            //connect to database
                            SqlConnection cn = new SqlConnection(connectString);
                            //open database connection
                            cn.Open();
                            //insert query to add entry
                            string insertQuery = "INSERT INTO Buyers VALUES('" + name + "', '" + shares + "', '" + datePurchased + "', '" + shareType + "')";
                            //Creating sql command to execute
                            SqlCommand command = new SqlCommand(insertQuery, cn);
                            //execute query
                            command.ExecuteNonQuery();

                            //Selection query to check whether have enough shares
                            string selectionQuery = "";
                            if (shareType == "common")
                            {
                                selectionQuery = "SELECT common FROM Shares";
                            }
                            else
                            {
                                selectionQuery = "SELECT preferred FROM Shares";
                            }
                            //Creating sql command to execute
                            SqlCommand secondCommand = new SqlCommand(selectionQuery, cn);
                            //checking shares available
                            int availableShares = Convert.ToInt32(secondCommand.ExecuteScalar());
                            availableShares = availableShares - shares;
                            if (availableShares < 0)
                            {
                                MessageBox.Show("ERROR, We dont have that many shares available");
                            }
                            else
                            {
                                string updateQuery = "";
                                //checking to see which one to update for share type
                                if (shareType == "common")
                                {
                                    //update common shares
                                    updateQuery = "UPDATE Shares SET common = '" + availableShares + "'";
                                    SqlCommand thirdCommand = new SqlCommand(updateQuery, cn);
                                    thirdCommand.ExecuteScalar();
                                }
                                else
                                {
                                    //update preferred shares
                                    updateQuery = "UPDATE Shares SET preferred = '" + availableShares + "'";
                                    SqlCommand thirdCommand = new SqlCommand(updateQuery, cn);
                                    thirdCommand.ExecuteScalar();
                                }
                                //showing that entry was created 
                                MessageBox.Show("Successfully added a share purchase");
                                //clear values for next entry
                                txtName.Text = "";
                                txtShares.Text = "";
                                dtPicker.SelectedDate = null;
                                cn.Close();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Please Select a purchase date");
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("ERROR, Number of shares must be numeric integer");
                        txtShares.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Please enter Buyer Name");
                }
                
            }
            //catching any sort of exceptions that might occur.
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //if tab is changed 
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //check to see which tab is selected 
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            const string V = "View Summary";
            const string E = "View Entries";
            switch (tabItem)
            {
                case V:
                    try
                    {
                        //connectString for database
                        string connectString = Properties.Settings.Default.connect_string;
                        //connect to database
                        SqlConnection cn = new SqlConnection(connectString);
                        //open database connection
                        cn.Open();
                        //query to get shares
                        string retrieveSharesSoldQuery = "SELECT SUM(shares) FROM Buyers WHERE shareType = 'common'";
                        //create command for execution
                        SqlCommand fourthCommand = new SqlCommand(retrieveSharesSoldQuery, cn);
                        int soldShares = Convert.ToInt32(fourthCommand.ExecuteScalar());
                        txtCommon.Text = soldShares.ToString();
                        //------------------------------------------------------------------
                        //query to get preffered shares sold
                        string preferredSharesSoldQuery = "SELECT SUM(shares) FROM Buyers WHERE shareType = 'preferred'";
                        //create command for execution
                        SqlCommand fifthCommand = new SqlCommand(preferredSharesSoldQuery, cn);
                        int preferredSoldShares = Convert.ToInt32(fifthCommand.ExecuteScalar());
                        txtPreferred.Text = preferredSoldShares.ToString();
                        //------------------------------------------------------------------
                        //query to get common shares available
                        string commonSharesAvailable = "SELECT common FROM Shares";
                        //create command for execution
                        SqlCommand sixthCommand = new SqlCommand(commonSharesAvailable, cn);
                        int common_SharesAvailable = Convert.ToInt32(sixthCommand.ExecuteScalar());
                        txtCommonAvailable.Text = common_SharesAvailable.ToString();
                        //------------------------------------------------------------------
                        //query to get preferred shares available
                        string preferredSharesAvailable = "SELECT preferred FROM Shares";
                        //create command for execution
                        SqlCommand seventhCommand = new SqlCommand(preferredSharesAvailable, cn);
                        int preferred_SharesAvailable = Convert.ToInt32(seventhCommand.ExecuteScalar());
                        txtPreferredAvailable.Text = preferred_SharesAvailable.ToString();
                        //------------------------------------------------------------------
                        //value for amount of prefered sold with preferred share price method
                        int preferredRevenue = preferredSoldShares * getPreferredSharePrice();
                        //value for amount of common sold with common share price method
                        int commonRevenue = soldShares * getCommonSharePrice();
                        //total revenue value
                        int totalRevenue = preferredRevenue + commonRevenue;
                        txtRevenue.Text = totalRevenue.ToString();
                    }
                    //catching any sort of exceptions that might occur.
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;
                case E:
                    try
                    {
                        //connectString for database
                        string connectString = Properties.Settings.Default.connect_string;
                        //connect to database
                        SqlConnection cn = new SqlConnection(connectString);
                        //open database connection
                        cn.Open();
                        //Query to view table
                        string selectionQuery = "SELECT * FROM Buyers";
                        //Creating a command and passing the SqlCommand method the query and the connection.
                        SqlCommand command = new SqlCommand(selectionQuery, cn);
                        //SQL Adapter 
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        //This datatable is being linked with the Buyers table.
                        DataTable dt = new DataTable("Buyers");
                        //retrieve data from a data source and populate tables within a dataset.
                        sda.Fill(dt);
                        //buyersGrid is being bound to the datatable
                        buyersGrid.ItemsSource = dt.DefaultView;
                    }
                    //catching any sort of exceptions that might occur.
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;
            }
        }
        //method to get common share price
        public static int getCommonSharePrice()
        {
            //connectString for database
            string connectString = Properties.Settings.Default.connect_string;
            //connect to database
            SqlConnection cn = new SqlConnection(connectString);
            //open database connection
            cn.Open();
            //query to get date from database
            string dateFromDatabase = "Select datePurchased FROM Buyers WHERE name='Najeeb'";
            SqlCommand eightCommand = new SqlCommand(dateFromDatabase, cn);
            string dateFromDB = eightCommand.ExecuteScalar().ToString();
            DateTime dateExtractedFromDatabase = DateTime.Parse(dateFromDB);
            double day = (dateExtractedFromDatabase - new DateTime(1990, 1, 1)).TotalDays;
            int days = Convert.ToInt32(day);
            Random rnd = new Random(days);
            int id = rnd.Next(1, 200);
            return id;
        }
        //method to get preferred shared price 
        public static int getPreferredSharePrice()
        {
            //connectString for database
            string connectString = Properties.Settings.Default.connect_string;
            //connect to database
            SqlConnection cn = new SqlConnection(connectString);
            //open database connection
            cn.Open();
            //query to get date from database
            string dateFromDatabase = "Select datePurchased FROM Buyers WHERE name='Najeeb'";
            SqlCommand ninthCommand = new SqlCommand(dateFromDatabase, cn);
            string dateFromDB = ninthCommand.ExecuteScalar().ToString();
            DateTime dateExtractedFromDatabase = DateTime.Parse(dateFromDB);
            double day = (dateExtractedFromDatabase - new DateTime(1990, 1, 1)).TotalDays;
            int days = Convert.ToInt32(day);
            Random rnd = new Random(days);
            int id = rnd.Next(20, 1000);
            return id;
        }

    }
}
