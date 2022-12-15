public class DexEntryData
{
    public int dex;
    public PokemonData pokemon = null;
    public bool seen = false;
    public bool caught = false;

    public DexEntryData(int dex) => this.dex = dex;
}
