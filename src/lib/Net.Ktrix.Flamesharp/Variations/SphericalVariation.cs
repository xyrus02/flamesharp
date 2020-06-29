namespace Net.Ktrix.Flamesharp.Variations
{
	[Variation("spherical")]
	public class SphericalVariation : Variation
	{
		public override void Calculate(CalculationState calculationState)
		{
			var r2 = calculationState.Input.LengthSquared + 10e-30;
			calculationState.Output += Weight / r2 * calculationState.Input;
		}
	}
}