namespace mrbBase
{
    public class PowerSubEntry
    {
        public int nIDPower = -1;
        public int Power = -1;
        public int Powerset = -1;
        public bool StatInclude;

        public void Assign(PowerSubEntry iPowerSubEntry)
        {
            Powerset = iPowerSubEntry.Powerset;
            Power = iPowerSubEntry.Power;
            nIDPower = iPowerSubEntry.nIDPower;
            StatInclude = iPowerSubEntry.StatInclude;
        }
    }
}