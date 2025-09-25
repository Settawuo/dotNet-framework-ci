using System;
using FBBPAYG_GenFile_DisneyPB.CompositionRoot;

namespace FBBPAYG_GenFile_DisneyPB
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Bootstrapper.Bootstrap();
                FBBPAYGGenFileDisneyPBBatch instance = Bootstrapper.GetInstance<FBBPAYGGenFileDisneyPBBatch>();
                if (!instance.CheckProcessBatch())
                    return;
                instance.GendataFiles();

            }catch (Exception ex)
            {

            }
        }
    }
}
