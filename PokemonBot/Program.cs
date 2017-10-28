namespace PokemonBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Pokebot().MainAsync().GetAwaiter().GetResult();
        }
    }
}