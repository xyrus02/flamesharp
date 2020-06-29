namespace Net.Ktrix.Flamesharp.Variations
{
	[Variation("linear")]
	public class LinearVariation : Variation
	{
		public override void Calculate(CalculationState calculationState)
		{
			calculationState.Output += Weight * calculationState.Input;
		}
	}
}