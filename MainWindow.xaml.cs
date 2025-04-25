using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using Telefonkonyv;

namespace WPFEFCMF_Telefonkonyv
{
    public partial class MainWindow : Window
    {

        cnTelefonkonyv cn;
        public MainWindow()
        {
            InitializeComponent();
            cn = new cnTelefonkonyv();
            DBInic();
        }
        private void DBInic()
        {
            cn.Database.EnsureCreated();
            if (cn.Személyek == null) return;
            if (!cn.Személyek.Any())
            {
                KezdőAdatok();
            }
            AdatKiír();
        }

        private void KezdőAdatok()
        {
            var h1 = new Helyseg { Irányítószám = 6000, Név = "Kecskemét" };
            var h2 = new Helyseg { Irányítószám = 3860, Név = "Encs" };
            var sz1 = new Szemely { Név = "Gipsz Jakab", Cím = "Izsáki út 10.", Helység = h1 };
            var sz2 = new Szemely { Név = "Programozó Ubul", Cím = "Nagy utca 7.", Helység = h2 };
            h1.Személyek.Add(sz1);
            h2.Személyek.Add(sz2);
            var szám1 = new Szam { SzámSztring = "+36-99-9999999" };
            var szám2 = new Szam { SzámSztring = "+36-76-9998899" };
            var szám3 = new Szam { SzámSztring = "+36-88-9978991" };
            sz1.Számok.Add(szám1);
            sz1.Számok.Add(szám3);
            sz2.Számok.Add(szám2);
            sz2.Számok.Add(szám3);
            szám1.Személyek.Add(sz1);
            szám2.Személyek.Add(sz2);
            szám3.Személyek.Add(sz1);
            szám3.Személyek.Add(sz2);
            cn.Személyek.AddRange([sz1, sz2]);
            cn.Helységek.AddRange([h1, h2]);
            cn.Számok.AddRange([szám1, szám2, szám3]);
            cn.SaveChanges();
        }

        private void AdatKiír()
        {
            var s = "";
            foreach (var p in
                cn.Személyek.Include(pe => pe.Helység).Include(pe => pe.Számok).ToList())
            {
                s += p.Név + ", " + (p.Helység != null ? p.Helység.Irányítószám + ", " : "")
                        + (p.Helység != null ? p.Helység.Név + ", " : "") + p.Cím + ", "
                        + p.Számok.Aggregate("", (c, a) => (c.Length > 0 ? c + ", " : "")
                        + a.SzámSztring) + "\n";
            }
            MessageBox.Show(s);
        }

        private void mi_KilépésClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Összecsuk()
        {
            dgMind.Visibility = Visibility.Collapsed;
            dgHelységek.Visibility = Visibility.Collapsed;
            grHelység.Visibility = Visibility.Collapsed;
            grSzám.Visibility = Visibility.Collapsed;
            grSzemély.Visibility = Visibility.Collapsed;
        }

        private void mi_HelységekClick(object sender, RoutedEventArgs e)
        {
            Összecsuk();
            dgHelységek.Visibility = Visibility.Visible;
            dgHelységek.ItemsSource = cn.Helységek.ToList();
        }

        private void mi_MindenClick(object sender, RoutedEventArgs e)
        {
            Összecsuk();
            dgMind.Visibility = Visibility.Visible;
            dgMind.ItemsSource = cn.Személyek.Include(p => p.Helység).Include(
                p => p.Számok).ToList();
        }

        private void mi_ÚMHelységekClick(object sender, RoutedEventArgs e)
        {
            Összecsuk();
            grHelység.Visibility = Visibility.Visible;
            grHelység.DataContext = cn.Helységek.Include(h => h.Személyek).ToList();
            cbIrányítószám.SelectedIndex = 0;
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var h = ((ComboBox)sender).SelectedItem as Helyseg;
            if (h == null) return;
            tbHelységnév.Text = h.Név;
            tbIrányítószám.Text = h.Irányítószám.ToString();
        }
        private void btÚMVissza_Click(object sender, RoutedEventArgs e)
        {
            tbHelységnév.Text = "";
            tbIrányítószám.Text = "";
            grHelység.Visibility = Visibility.Collapsed;
        }

        private void btÚMMentés_Click(object sender, RoutedEventArgs e)
        {
            if (!NévIrányítószámEllenőriz(false)) return;
            var h = cbNév.SelectedItem as Helyseg;
            if (h == null) return;
            h.Név = tbHelységnév.Text;
            h.Irányítószám = short.Parse(tbIrányítószám.Text);
            h.Név = tbHelységnév.Text;
            cn.SaveChanges();
            MessageBox.Show("Módosítások elmentve!");
            grHelység.DataContext = null;
            grHelység.DataContext = cn.Helységek.Include(h => h.Személyek).ToList();
            cbNév.SelectedItem = h;
        }

        bool NévIrányítószámEllenőriz(bool ÚjIrszKell = true)
        {
            if (tbIrányítószám.Text.Length == 0)
            {
                MessageBox.Show("Kérlek add meg a helység irányítószámát!");
                return false;
            }
            var res = short.TryParse(tbIrányítószám.Text, out short irsz);
            if (!res || (res && irsz < 1))
            {
                MessageBox.Show("Az irányítószám nem érvényes!");
                return false;
            }
            if (ÚjIrszKell && cn.Helységek.Any(h => h.Irányítószám == irsz))
            {
                MessageBox.Show("Ez az irányítószám már szerepel az adatbázisban!");
                return false;
            }
            if (tbHelységnév.Text == "")
            {
                MessageBox.Show("Kérlek add meg a helység nevét!");
                return false;
            }
            return true;
        }
        private void btÚMÚjMentés_Click(object sender, RoutedEventArgs e)
        {
            if (!NévIrányítószámEllenőriz()) return;
            var újh = new Helyseg
            {
                Irányítószám = short.Parse(tbIrányítószám.Text),
                Név = tbHelységnév.Text
            };
            cn.Helységek.Add(újh);
            cn.SaveChanges();
            MessageBox.Show("Helység mentve!");
            tbHelységnév.Text = "";
            tbIrányítószám.Text = string.Empty;
            grHelység.DataContext = null;
            grHelység.DataContext = cn.Helységek.Include(h => h.Személyek).ToList();
            cbIrányítószám.SelectedItem = újh;
        }

        private void mi_ÚMTelefonszámokClick(object sender, RoutedEventArgs e)
        {
            Összecsuk();
            grSzám.Visibility = Visibility.Visible;
            grSzám.DataContext = cn.Számok.Include(sz => sz.Személyek).ToList();
            cbSzámok.SelectedIndex = 0;
        }

        private void cbSzám_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var n = ((ComboBox)sender).SelectedItem as Szam;
            if (n == null) return;
            tbÚjSzám.Text = n.SzámSztring;
        }

        private void btTelefonszámMentés_Click(object sender, RoutedEventArgs e)
        {
            if (tbÚjSzám.Text.Length < 3)
            {
                MessageBox.Show("Érvénytelen telefonszám!");
                return;
            }
            if (cn.Számok.Any(sz => sz.SzámSztring == tbÚjSzám.Text))
            {
                MessageBox.Show("A szám már szerepel az adatbázisban!");
                return;
            }
            var sz = cbSzámok.SelectedItem as Szam;
            if (sz == null) return;
            sz.SzámSztring = tbÚjSzám.Text;
            cn.SaveChanges();
            MessageBox.Show("Szám módosítva!");
            grSzám.DataContext = null;
            grSzám.DataContext = cn.Számok.Include(sz => sz.Személyek).ToList();
            cbSzámok.SelectedItem = sz;
        }

        private void btÚjSzámMentés_Click(object sender, RoutedEventArgs e)
        {
            if (tbÚjSzám.Text.Length < 3)
            {
                MessageBox.Show("Érvénytelen telefonszám!");
                return;
            }
            if (cn.Számok.Any(sz => sz.SzámSztring == tbÚjSzám.Text))
            {
                MessageBox.Show("A szám már szerepel az adatbázisban!");
                return;
            }
            Szam sz = new Szam { SzámSztring = tbÚjSzám.Text };
            cn.Számok.Add(sz);
            cn.SaveChanges();
            MessageBox.Show("Szám mentve!");
            grSzám.DataContext = null;
            grSzám.DataContext = cn.Számok.Include(sz => sz.Személyek).ToList();
            cbSzámok.SelectedItem = 0;
        }

        private void btTelefonszámVissza_Click(object sender, RoutedEventArgs e)
        {
            grSzám.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            DBLecsatol();
        }

        private void DBLecsatol()
        {
            cn.Database.OpenConnection();
            var connection = cn.Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            string[] commands =
            { "USE master",
        $"ALTER DATABASE [{connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
        $"ALTER DATABASE [{connection.Database}] SET OFFLINE WITH ROLLBACK IMMEDIATE",
        $"EXEC sp_detach_db '{connection.Database}'"
      };
            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection as SqlConnection;
                foreach (string command in commands)
                {
                    sqlCommand.CommandText = command;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private void btÚMHTöröl_Click(object sender, RoutedEventArgs e)
        {
            var h = cbIrányítószám.SelectedItem as Helyseg;
            if (h == null) return;
            foreach (var sz in h.Személyek)
                sz.Helység = null;
            cn.Helységek.Remove(h);
            cn.SaveChanges();
            MessageBox.Show("Helység törölve!");
            grHelység.DataContext = cn.Helységek.Include(c => c.Személyek).ToList();
            cbIrányítószám.SelectedIndex = 0;
            cbIrányítószám.UpdateLayout();
        }

        private void btÚMTTöröl_Click(object sender, RoutedEventArgs e)
        {
            var szám = cbSzámok.SelectedItem as Szam;
            if (szám == null) return;
            foreach (var személy in szám.Személyek)
                személy.Számok.Remove(szám);
            cn.Számok.Remove(szám);
            cn.SaveChanges();
            MessageBox.Show("Szám törölve!");
            grSzám.DataContext = null;
            grSzám.DataContext = cn.Számok.Include(szám => szám.Személyek).ToList();
            cbSzámok.SelectedIndex = 0;
        }








    }
}