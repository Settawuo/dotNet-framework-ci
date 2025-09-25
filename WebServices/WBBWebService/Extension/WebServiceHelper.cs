using System;
using System.ServiceModel;

namespace WBBWebService.Extension
{
    internal static class WebServiceHelper<T> where T : class
    {
        public static void Call(Action<T> action)
        {
            ChannelFactory<T> factory = new ChannelFactory<T>("*");
            T serviceClient = factory.CreateChannel();

            var success = false;

            try
            {
                action(serviceClient);
                ((IClientChannel)serviceClient).Close();
                factory.Close();
                success = true;
            }
            catch (CommunicationException cex)
            {
                throw cex;
            }
            catch (TimeoutException toex)
            {
                throw toex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!success)
                {
                    ((IClientChannel)serviceClient).Abort();
                    factory.Abort();
                }
            }
        }
    }
}