class Buyer
{
    public string Name { get; set; }
    public string Contact { get; set; }
    public Brand DesiredBrand { get; set; }
    public decimal MaxPrice { get; set; }

    public Dictionary<Part, Condition> DesiredParts { get; set; }
}


