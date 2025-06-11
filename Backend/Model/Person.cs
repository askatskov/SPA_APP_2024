namespace Backend.Model
{
	public record Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public DateTime Date { get; set; }
	}
}