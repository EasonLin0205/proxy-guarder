using System;
using ProxyGuarder;

internal static class ProbeTest
{
    private static int Main()
    {
        try
        {
            var names = NetworkManager.GetConnectedNetworkNames();
            Console.WriteLine("COUNT=" + names.Count);
            foreach (string n in names)
            {
                Console.WriteLine("NAME=" + n);
                Console.Write("CP=");
                foreach (char c in n)
                {
                    Console.Write(((int)c).ToString("X4") + " ");
                }
                Console.WriteLine();
            }
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR=" + ex);
            return 1;
        }
    }
}
