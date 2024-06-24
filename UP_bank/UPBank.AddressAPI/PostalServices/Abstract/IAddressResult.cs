namespace UPBank.AddressAPI.PostalServices.Abstract
{
    public interface IAddressResult
    {
        public string Street { get; set; }
        public string StreetType { get; set; }
        public string Zipcode { get; set; }
        public string Neighborhood { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}
