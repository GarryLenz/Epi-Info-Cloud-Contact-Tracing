﻿using System.Collections.Generic;
using System.Linq;
using Epi.Common.Core.DataStructures;
using Epi.DataPersistence.Constants;
using Epi.FormMetadata.DataStructures;

namespace Epi.DataPersistenceServices.CosmosDB
{
    public partial class CosmosDBCRUD
    {
        private const string LT = "<";
        private const string LE = "<=";
        private const string EQ = "=";
        private const string NE = "!=";
        private const string GT = ">";
        private const string GE = ">=";
        private const string SELECT = "SELECT ";
        private const string ORDER_BY = " ORDER BY ";
        private const string DESC = " DESC ";
        private const string FROM = " FROM ";
        private const string WHERE = " WHERE ";
        private const string AND = " AND ";
        private const string Alias = "`@`";

        private const string FRP_ = "FormResponseProperties.";
        private const string FRP_RecStatus = FRP_ + "RecStatus";
        private const string FRP_ResponseQA_ = FRP_ + "ResponseQA.";
        private const string FRP_UserOrgId = FRP_ + "UserOrgId";
        private const string FRP_FirstSaveTime = FRP_ + "FirstSaveTime";

        private const string udf_wildCardCompare = "udf.WildCardCompare";
        private const string udf_sharingRules = "udf.SharingRules";

        private string AssembleSelect(string collectionName, params string[] columnNames)
        {
            string columnList;
            if (columnNames.Length == 1 && columnNames[0] == "*")
            {
                columnList = "*";
            }
            else
            {
                columnList = collectionName + '.' + string.Join(", " + collectionName + '.', columnNames);
            }
            return columnList;
        }

        private string AssembleParentSelect(string collectionName, params string[] columnNames)
        {
            string columnList;
            if (columnNames.Length == 1 && columnNames[0] == "*")
            {
                columnList = "*";
            }
            else
            {
                columnList = collectionName + string.Join("," + collectionName, columnNames);
            }
            return columnList;
        }

        private string AssembleParentQASelect(string formName, string[] columnNames)
        {
            string query = "{"
                            + AssembleParentSelect(null, columnNames.Select(g => g.ToLower() + ":" + formName + "." + FRP_ResponseQA_ + g.ToLower()).ToArray())
                            + "} AS ResponseQA "
                            + FROM + formName;
            return query;
        }

        private string AssembleExpressions(string collectionName, params string[] expressions)
        {
            string where;
            where = string.Join(" ", expressions.Select(e => e.ToString().Replace(Alias, collectionName)));
            return where;
        }
        private string AssembleAndExpressions(string collectionName, params string[] expressions)
        {
            string where;
            where = AND + string.Join(" ", expressions.Select(e => e.ToString().Replace(Alias, collectionName)));
            return where;
        }

        private static string Expression(string left, string relational_operator, object right, string and = null)
        {
            string expression;

            if (right == null)
            {
                expression = string.Format("{0}.{1} {2} null", Alias, left, relational_operator);
            }
            else
            {
                if (right is string == false)
                {
                    expression = string.Format("{0}.{1} {2} {3}", Alias, left, relational_operator, right);
                }
                else if (left.Contains(FRP_ResponseQA_))
                {
                    expression = string.Format("{0}.{1} {2} \"{3}\"", Alias, left, relational_operator, right);
                }
                else
                {
                    expression = string.Format("{0}.{1} {2} \"{3}\"", Alias, left, relational_operator, right);
                }
            }

            return and == null ? expression : and + expression;
        }

        private static string ExpressionWithFunction(string left, string relational_operator, object right, string function)
        {
            string expression = string.Format("{0}({1}.{2}) {3} {4}", function, Alias, left, relational_operator, "\"" + right.ToString() + "\"");
            return expression;
        }

        private static string And_Expression(string left, string relational_operator, object right, bool excludeExpression = false)
        {
            var expression = excludeExpression ? string.Empty : Expression(left, relational_operator, right, AND);
            return expression;
        }

        private static string And_Expression(string left, string relational_operator, object right, string function)
        {
            var expression = function == null
                ? Expression(left, relational_operator, right)
                : ExpressionWithFunction(left, relational_operator, right, function);
            return AND + expression;
        }

        private static string And_SearchExpressions(KeyValuePair<FieldDigest, string>[] searchQualifiers)
        {
            string searchExpression = string.Empty;
            if (searchQualifiers == null || searchQualifiers.Length == 0) return string.Empty;
            foreach (var searchQualifier in searchQualifiers)
            {
                if (searchQualifier.Value.Contains('*') || searchQualifier.Value.Contains('?') || searchQualifier.Value.ToLowerInvariant().Contains("regex:"))
                {
                    var expression = And_Expression(FRP_ResponseQA_ + searchQualifier.Key.FieldName, EQ, searchQualifier.Value.ToLowerInvariant());
                    searchExpression += expression;
                }
            }
            return searchExpression;
        }

        private static string LOWER(string argument)
        {
            return string.Format("LOWER({0})", argument);
        }

        private class WildCardQualifier
        {
            public WildCardQualifier(string fieldName, string fieldPattern)
            {
                FieldName = fieldName;
                FieldPattern = fieldPattern;
            }
            public string FieldName;
            public string FieldPattern;
        }

        private string GenerateResponseGridQuery(string collectionAlias, string formId, List<string> formPoperties, 
            string[] columnlist, KeyValuePair<FieldDigest, string>[] searchQualifiers,
            ResponseAccessRuleContext responseAccessRuleContext, string querySetToken)
        {
            string SelectColumnList = string.Empty;


            var SelectFormPoperties = AssembleSelect(collectionAlias, formPoperties.Select(g => FRP_ + g).ToArray());

            if (columnlist != null && columnlist.Length > 0)
            {
                // convert column list to this format {patientname1: Zika.FormResponseProperties.ResponseQA.patientname1} as ResponseQA
                SelectColumnList = AssembleParentQASelect(collectionAlias, columnlist);
            }

            string query = null;

            var trailingExpressions = new List<string>();
            trailingExpressions.Add(And_Expression(FRP_RecStatus, NE, RecordStatus.Deleted));
            if (!string.IsNullOrWhiteSpace(querySetToken)) trailingExpressions.Add(And_Expression(FRP_FirstSaveTime, LE, querySetToken));

            if (searchQualifiers != null && searchQualifiers.Length > 0)
            {
                List<WildCardQualifier> wildCardQualifiers = new List<WildCardQualifier>();
                var searchExpressionList = searchQualifiers.Select(x => And_SearchExpression(x.Key.FieldName, x.Value, wildCardQualifiers)).Where(x => x != null).ToList();
                var wildCardSearchExpression = And_WildCardSearchExpression(wildCardQualifiers);
                if (wildCardSearchExpression != null) searchExpressionList.Add(wildCardSearchExpression);
                if (responseAccessRuleContext != null && responseAccessRuleContext.IsSharable)
                    query = SELECT
                                   + SelectFormPoperties + ","
                                   + AssembleSelect(collectionAlias, "_ts,")
                                   + SelectColumnList
                                   + WHERE
                                   + AssembleAcessRuleQualifier(collectionAlias, responseAccessRuleContext)
                                   + AssembleExpressions(collectionAlias, searchExpressionList.ToArray())
                                   + AssembleExpressions(collectionAlias, trailingExpressions.ToArray());
                else
                    query = SELECT
                               + SelectFormPoperties + ","
                               + AssembleSelect(collectionAlias, "_ts,")
                               + SelectColumnList
                               + WHERE                               
                               + AssembleExpressions(collectionAlias, searchExpressionList.ToArray()).Remove(0, 4)
                               + AssembleExpressions(collectionAlias, trailingExpressions.ToArray());
            }
            else if (responseAccessRuleContext != null && responseAccessRuleContext.IsSharable)           
            {
                query = SELECT
                               + SelectFormPoperties + ","
                               + AssembleSelect(collectionAlias, "_ts,")
                               + SelectColumnList
                               + WHERE
                               + AssembleAcessRuleQualifier(collectionAlias, responseAccessRuleContext)
                               + AssembleExpressions(collectionAlias, trailingExpressions.ToArray());
            }
            else
            {
                query = SELECT
                              + SelectFormPoperties + ","
                              + AssembleSelect(collectionAlias, "_ts,")
                              + SelectColumnList
                              + WHERE                             
                              + AssembleExpressions(collectionAlias, trailingExpressions.ToArray()).Remove(0,4);

            }

            return query;
        }

        private string And_SearchExpression(string fieldName, string fieldValue, List<WildCardQualifier> wildCardQualifiers)
        {
            string searchExpression = null ;
            if (fieldValue.Contains('*') || fieldValue.Contains('?') || fieldValue.StartsWith("~") || fieldValue.ToLowerInvariant().Contains("regex:"))
            {
                wildCardQualifiers.Add(new WildCardQualifier(fieldName, fieldValue.ToLowerInvariant()));
            }
            else
            {
                searchExpression = And_Expression(FRP_ResponseQA_ + fieldName, EQ, fieldValue);
            }
            return searchExpression;
        }

        private string And_WildCardSearchExpression(List<WildCardQualifier> wildCardQualifiers)
        {
            string wildCardSearchExpression = null;
            if (wildCardQualifiers.Count() > 0)
            {
                wildCardSearchExpression = AND + udf_wildCardCompare + "(" + string.Join(",", wildCardQualifiers.Select(q => Alias + "." + FRP_ResponseQA_ + q.FieldName + "," + "\"" + q.FieldPattern + "\"").ToArray() ) + ")";
            }
            return wildCardSearchExpression;
        }

        private string AssembleAcessRuleQualifier(string collectionAlias, ResponseAccessRuleContext ruleContext)
        {
            string qualifier = string.Empty;
            if (ruleContext != null && ruleContext.IsSharable)
            {
                var parameters = new string[]
                {
                ruleContext.RuleId.ToString(),
                ruleContext.IsHostOrganizationUser.ToString().ToLower(),
                ruleContext.UserOrganizationId.ToString(),
                collectionAlias + "." + FRP_UserOrgId
                };

                qualifier = udf_sharingRules + "(" + string.Join(",", parameters) + ")";
            }

            return qualifier;
        }
    }
}
