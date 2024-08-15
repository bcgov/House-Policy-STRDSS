using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;

namespace StrDss.Service.Bceid
{
    public class LoggingMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // Log the entire SOAP request
            Console.WriteLine("SOAP Request: {0}", request.ToString());

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Log the entire SOAP reply
            Console.WriteLine("SOAP Response: {0}", reply.ToString());
        }
    }

    public class LoggingEndpointBehavior : IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new LoggingMessageInspector());
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
        public void Validate(ServiceEndpoint endpoint) { }
    }
}
