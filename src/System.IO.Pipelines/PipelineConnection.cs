using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    public class PipelineConnection : IPipelineConnection
    {
        public PipelineConnection(PipelineFactory factory)
        {
            Input = factory.Create();
            Output = factory.Create();
        }
        
        IPipelineReader IPipelineConnection.Input => Input;
        IPipelineWriter IPipelineConnection.Output => Output;

        public PipelineReaderWriter Input { get; }

        public PipelineReaderWriter Output { get; }

        public void Dispose()
        {
            Input.CompleteReader();
            Input.CompleteWriter();
            Output.CompleteReader();
            Output.CompleteWriter();
        }
    }
}
