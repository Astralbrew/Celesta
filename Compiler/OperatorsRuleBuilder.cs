using System.Collections.Generic;

namespace Astralbrew.Celesta.Compiler
{
    internal class OperatorsRuleBuilder
    {
        List<string[]> OperatorLayers = new List<string[]>();
        public void AddLayer(params string[] layer) => OperatorLayers.Add(layer);


        public List<(string Key, string[] Pattern, string[] TreeBuild)> GetRules(string expressionKey, string terminalKey)
        {
            var result = new List<(string Key, string[] Pattern, string[] TreeBuild)>();
            string key0 = $"{expressionKey}0";
            result.Add((expressionKey, new string[] { key0 }, new string[0]));

            for (int i = 0; i < OperatorLayers.Count; i++)
            {
                result.AddRange(GetLayerRules(i, expressionKey, terminalKey));
            }

            string keyN = $"{expressionKey}{OperatorLayers.Count}";

            result.Add((keyN, new string[] { terminalKey }, new string[0]));

            return result;
        }

        List<(string Key, string[] Pattern, string[] TreeBuild)> GetLayerRules(int layer, string expressionKey, string terminalKey)
        {
            var result = new List<(string Key, string[] Pattern, string[] TreeBuild)>();
            string thisKey = $"{expressionKey}{layer}";
            string nextKey = $"{expressionKey}{layer + 1}";

            result.Add((thisKey,
                $"{nextKey} ?? {string.Join("|", OperatorLayers[layer])} {nextKey}".Split(' '),
                "^L*".Split(' ')));

            return result;
        }
    }
}
