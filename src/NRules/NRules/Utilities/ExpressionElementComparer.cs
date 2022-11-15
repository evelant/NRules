﻿using System.Collections.Generic;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Utilities
{
    internal class ExpressionElementComparer
    {
        private readonly ExpressionComparer _expressionComparer;

        public ExpressionElementComparer(RuleCompilerUnsupportedExpressionsHandling unsupportedExpressionsHandling)
        {
            _expressionComparer = new(unsupportedExpressionsHandling);
        }

        public bool AreEqual(NamedExpressionElement first, NamedExpressionElement second)
        {
            if (!Equals(first.Name, second.Name))
                return false;
            return _expressionComparer.AreEqual(first.Expression, second.Expression);
        }

        public bool AreEqual(ExpressionElement first, ExpressionElement second)
        {
            return _expressionComparer.AreEqual(first.Expression, second.Expression);
        }

        public bool AreEqual(
            IReadOnlyCollection<Declaration> firstDeclarations, IEnumerable<NamedExpressionElement> first,
            IReadOnlyCollection<Declaration> secondDeclarations, IEnumerable<NamedExpressionElement> second)
        {
            using var enumerator1 = first.GetEnumerator();
            using var enumerator2 = second.GetEnumerator();

            while (true)
            {
                var hasNext1 = enumerator1.MoveNext();
                var hasNext2 = enumerator2.MoveNext();

                if (hasNext1 && hasNext2)
                {
                    if (!AreParameterPositionsEqual(firstDeclarations, enumerator1.Current, secondDeclarations, enumerator2.Current))
                        return false;
                    if (!AreEqual(enumerator1.Current, enumerator2.Current))
                        return false;
                }
                else if (hasNext1 || hasNext2)
                {
                    return false;
                }
                else
                {
                    break;
                }
            }

            return true;
        }

        public bool AreEqual(
            IReadOnlyCollection<Declaration> firstDeclarations, IEnumerable<ExpressionElement> x,
            IReadOnlyCollection<Declaration> secondDeclarations, IEnumerable<ExpressionElement> y)
        {
            return x.Count() == y.Count()
                   && x.Zip(y, (first, second) => new { X = first, Y = second })
                       .All(o =>
                           AreParameterPositionsEqual(firstDeclarations, o.X, secondDeclarations, o.Y) &&
                           AreEqual(o.X, o.Y));
        }

        private bool AreParameterPositionsEqual(
            IEnumerable<Declaration> firstDeclarations, ExpressionElement firstElement,
            IEnumerable<Declaration> secondDeclarations, ExpressionElement secondElement)
        {
            var parameterMap1 = IndexMap.CreateMap(firstElement.Imports, firstDeclarations);
            var parameterMap2 = IndexMap.CreateMap(secondElement.Imports, secondDeclarations);

            return Equals(parameterMap1, parameterMap2);
        }
    }
}
