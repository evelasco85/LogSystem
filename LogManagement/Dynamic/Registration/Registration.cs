namespace LogManagement.Dynamic.Registration
{
    public interface IRegistration
    {
        string Name { get; }
    }

    public class Registration
    {
        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }
        }

        public Registration(string name)
        {
            _name = name;
        }
    }
}
