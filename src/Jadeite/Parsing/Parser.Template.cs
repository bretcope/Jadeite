
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public partial class Parser
    {
        private TemplateNode ParseTemplate()
        {
            var template = new TemplateNode();
            template.SetModelDefinition(ParseModelDefinition());
            template.SetDocument(ParseDocument());

            return template;
        }

        private ModelDefinitionNode ParseModelDefinition()
        {
            var model = new ModelDefinitionNode();
        }

        private DocumentNode ParseDocument()
        {
            //
        }
    }
}
