namespace RestSourceGenerator.Generators.Models
{
    public struct Configuration
    {
        public string Assembly { get; set; }
        public Request[] Domains { get; set; }
    }
}