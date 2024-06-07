namespace Billy.Api.Models
{
    public class CountryRoot
    {
        public Country[] Countries { get; set; }
    }

    public class Country : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool HasStates { get; set; }
        public bool HasFiniteStates { get; set; }
        public bool HasFiniteZipcodes { get; set; }
        public string Icon { get; set; }
        public string LocaleId { get; set; }
        public Locale Locale { get; set; }
        public string CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }

}
