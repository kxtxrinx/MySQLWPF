using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Linq;
using DI_UT1_Ejemplo24_MySQL_ContextMenu_Settings.Properties;
//add this input to work with MySQL!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using MySql.Data.MySqlClient;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DI_UT1_Ejemplo24_MySQL_ContextMenu_Settings
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Declaración de la conexión a la BBDD
        MySqlConnection dbconn = null;

        public MainWindow()
        {
            InitializeComponent();
            CrearConexionDB();
            CargarComboContinente();

        }


        private void CrearConexionDB()

        {
            string server = Settings.Default.server;
            string database = Settings.Default.database;
            string uid = "sql11651753";
            string password = "Z8VININtri";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" +

            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            dbconn = new MySqlConnection(connectionString);

        }

        private void CargarComboPais()
        {
            if (dbconn.State != System.Data.ConnectionState.Open)
            {
                dbconn.Open();
            }

            string query = "SELECT code, name FROM country WHERE continent='" + cmbContinent.SelectedValue + "' ORDER BY name ASC";
            //Creación del comando
            MySqlCommand command = new MySqlCommand(query, dbconn);
            //Creación del dataReader para después recuperar la información
            MySqlDataReader reader = command.ExecuteReader();
            //Lectura y almacenamiento de datos en un comboBox
            while (reader.Read())
            {
                //Cada vez que pasa por aquí, se añade un nuevo país al comboBox de países
                cmbCountry.Items.Add(new
                {
                    code = reader["code"].ToString(), //nombre de la columna del resultado SQL
                    name = reader["name"].ToString()
                });
            }
            //Indicamos el campo a mostrar y cuál el que tendrá el valor con el que trabajaremos
            cmbCountry.SelectedValuePath = "code";
            cmbCountry.DisplayMemberPath = "name";


            reader.Close();
            dbconn.Close();
        }

        private void CargarComboContinente()
        {

            try{
                //Abrimos la conexión

                dbconn.Open(); 


                string query = "SELECT DISTINCT continent FROM country ORDER BY continent ASC";
                //Creación del comando
                MySqlCommand command = new MySqlCommand(query, dbconn);
                //Creación del dataReader para después recuperar la información
                MySqlDataReader reader = command.ExecuteReader();
                //Lectura y almacenamiento de datos en un comboBox
                while (reader.Read())
                {
                    //Cada vez que pasa por aquí, se añade un nuevo continente
                    cmbContinent.Items.Add(new {
                        continent = reader["continent"].ToString() //coge la columna continent de todos los resultados devueltos del SELECT
                    });
                }
                //Indicamos el campo a mostrar y cuál el que tendrá el valor con el que trabajaremos
                cmbContinent.SelectedValuePath = "continent"; // si el usuario selecciona un elemento en el ComboBox, el valor de la propiedad "continent" de ese elemento se asignará como el valor seleccionado, que luego puedes recuperar y utilizar en tu código
                cmbContinent.DisplayMemberPath = "continent"; //significa que el texto visible para cada elemento en el ComboBox será el valor de la propiedad "continent".
                //En SelectedPath está la propiedad que se accederá al llamar cmbContinent.SelectedValue

                //Cierre del dataReader
                reader.Close();
                //Cierre de la conexión
                dbconn.Close();
            }
            catch(MySqlException e) //No es necesario el pseudónimo de la excepción si no la vamos a usar
            {
                MessageBox.Show(""+e, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbContinent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCountry.Items.Clear();
            CargarComboPais();
        }

        private void BuscarCiudades()
        {
            if (dbconn.State != System.Data.ConnectionState.Open)
            {
                dbconn.Open();
            }

            string query = "SELECT id, name, countrycode, district, population FROM city WHERE countrycode = '" + cmbCountry.SelectedValue + "' ORDER BY name ASC";

            MySqlCommand command = new MySqlCommand(query, dbconn);

            //Creamos un DataAdapter y ejecutamos el comando
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            //El dataadapter se llena con una datatable, y esta datatable la asociamos al datagrid

            //Introducimos los datos recuperados en una tabla
            DataTable dt = new DataTable();
            da.Fill(dt);

            //Asociamos la dataTable al dataGrid
            dtGCity.ItemsSource = dt.DefaultView;

            dbconn.Close();

            //Dejamos no visible la columna ID
            dtGCity.Columns[0].Visibility = Visibility.Collapsed;

            //Deshabilitar que se puedan seleccionar varios registros
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            BuscarCiudades();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("code is " + cmbCountry.SelectedValue.ToString() + " while the display member is ");

            AddCityWindow addCityWindow = new AddCityWindow(this.dbconn);
            //addCityWindow.ShowDialog();

            if(cmbCountry.SelectedItem != null)
            {
                addCityWindow.SetCodigoPais(cmbCountry.SelectedValue.ToString());
            }
            if (addCityWindow.ShowDialog() == true)
            {
                BuscarCiudades();
            } 
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            EliminarCiudad();
        }

        private void EliminarCiudad()
        {
            if(dtGCity.SelectedItems.Count > 0)
            {
                DataRowView city = (DataRowView)dtGCity.SelectedItems[0];
                string idCity = city[0].ToString();
                string nameCity = city[1].ToString();

                if (MessageBox.Show("are u sure u wanna del" + nameCity +"????", "Care", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
                {
                    try
                    {
                        //Abrimos la connex
                        dbconn.Open();
                        //Preparamos la sentencia SQL
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = dbconn;

                        //enumerar siempre las columnas
                        string query = "DELETE FROM city WHERE ID =" + idCity;

                        cmd.CommandText = query;
                        //Ejecutamos la consulta
                        cmd.ExecuteNonQuery();

                        //Cerramos la consulta
                        dbconn.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Check your connection with the DB" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    BuscarCiudades(); //Para que se actualice tras eliminarlo
                }
            }
        }
    }



}