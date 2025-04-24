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
            grHelység.Visibility = Visibility.Collapsed;
            grSzám.Visibility = Visibility.Collapsed;
            dgMind.Visibility = Visibility.Collapsed;
            dgHelységek.Visibility = Visibility.Collapsed;
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
    }
}