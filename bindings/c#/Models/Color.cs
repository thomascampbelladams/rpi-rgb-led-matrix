public struct Color
{
    public Color(int r, int g, int b)
    {
        R = (byte)r;
        G = (byte)g;
        B = (byte)b;
    }
    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
    public Color(uint hexValue)
    {
        R = (byte)((hexValue & 0xff0000) >> 0x10);
        G = (byte)((hexValue & 0xff00) >> 8);
        B = (byte)(hexValue & 0xff);
    }
    public byte R;
    public byte G;
    public byte B;
}