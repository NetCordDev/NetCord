using NetCord;

namespace ColorTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void ColorTest()
    {
        int rgb = 0x0000ff;
        while (true)
        {
            Color color = new(rgb);
            Assert.AreEqual(rgb, color.RawValue);
            Assert.AreEqual((byte)(rgb >> 16), color.Red);
            Assert.AreEqual((byte)(rgb >> 8), color.Green);
            Assert.AreEqual((byte)rgb, color.Blue);

            if (rgb == 0xff0000)
                break;
            rgb <<= 8;
        }
    }

    [TestMethod]
    public void ColorTest2()
    {
        byte r = 32, g = 36, b = 12;
        int rgb = (r << 16) | (g << 8) | b;
        Color color = new(r, g, b);
        Assert.AreEqual(rgb, color.RawValue);
        Assert.AreEqual(r, color.Red);
        Assert.AreEqual(g, color.Green);
        Assert.AreEqual(b, color.Blue);
    }
}
