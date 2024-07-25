using Bogus;
using Bogus.Distributions.Gaussian;
public static class RNG
{
    private static readonly Faker _faker = new Faker();
    public static string GenerateProduct() => _faker.Commerce.Product();
    public static int RandomQuantity() => _faker.Random.Int(1,int.MaxValue / 8);
}