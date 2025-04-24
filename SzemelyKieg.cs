namespace Telefonkonyv
{
    public partial class Szemely
    {
        public string? HelységNév => Helység?.Név;
        public short? HelységIrányítószám => Helység?.Irányítószám;
        public string TelefonszámLista => Számok.Aggregate("",
            (c, a) => c + (c.Length > 0 ? ", " : "") + a.SzámSztring);
    }
}