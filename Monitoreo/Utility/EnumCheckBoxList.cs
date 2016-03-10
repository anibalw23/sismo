using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Monitoreo.Utility
{
    public static class EnumCheckBoxList
    {
        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        public static MvcHtmlString CheckBoxListForEnum<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return CheckBoxListForEnum(htmlHelper, expression, null, true);
        }

        public static MvcHtmlString CheckBoxListForEnumTD<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return CheckBoxListForEnumTD(htmlHelper, expression, null, true);
        }

        public static MvcHtmlString CheckBoxListForEnum<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return CheckBoxListForEnum(htmlHelper, expression, htmlAttributes, true);
        }

        public static MvcHtmlString CheckBoxListForEnumTD<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return CheckBoxListForEnumTD(htmlHelper, expression, htmlAttributes, true);
        }

        public static MvcHtmlString CheckBoxListForEnum<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression, object htmlAttributes, bool sortAlphabetically)
        {
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullBindingName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var fieldId = TagBuilder.CreateSanitizedId(fullBindingName);

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var value = metadata.Model;

            // Get all enum values
            //IEnumerable values = Enum.GetValues(typeof(TValue)).Cast<TValue>();
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            // Sort them alphabetically by enum name
            //if (sortAlphabetically)
            //    values = values.OrderBy(i => i.ToString());

            // Create checkbox list
            var sb = new StringBuilder();
            foreach (var item in values)
            {
                TagBuilder builder = new TagBuilder("input");
                long targetValue = Convert.ToInt64(item);
                long flagValue = Convert.ToInt64(value);

                if ((targetValue & flagValue) == targetValue)
                    builder.MergeAttribute("checked", "checked");

                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", item.ToString());
                builder.MergeAttribute("name", fieldId);

                // Add optional html attributes
                if (htmlAttributes != null)
                    builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

                string innerHtml = Resources.T.ResourceManager.GetString(item.ToString());

                builder.InnerHtml = !string.IsNullOrEmpty(innerHtml) ? innerHtml : item.ToString();

                sb.Append(String.Format("<div class=\"checkbox\"><label>{0}</label></div>", builder.ToString(TagRenderMode.Normal)));
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString CheckBoxListForEnumTD<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression, object htmlAttributes, bool sortAlphabetically)
        {
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullBindingName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var fieldId = fullBindingName;// TagBuilder.CreateSanitizedId(fullBindingName);

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var value = metadata.Model;

            // Get all enum values
            //IEnumerable values = Enum.GetValues(typeof(TValue)).Cast<TValue>();
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            // Sort them alphabetically by enum name
            //if (sortAlphabetically)
            //    values = values.OrderBy(i => i.ToString());

            // Create checkbox list
            var sb = new StringBuilder();
            foreach (var item in values)
            {
                TagBuilder builder = new TagBuilder("input");
                long targetValue = Convert.ToInt64(item);
                long flagValue = Convert.ToInt64(value);

                if ((targetValue & flagValue) == targetValue)
                    builder.MergeAttribute("checked", "checked");

                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", item.ToString());
                builder.MergeAttribute("name", fieldId);

                // Add optional html attributes
                if (htmlAttributes != null)
                    builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

                //string innerHtml = Resources.T.ResourceManager.GetString(item.ToString());

                //builder.InnerHtml = !string.IsNullOrEmpty(innerHtml) ? innerHtml : item.ToString();

                sb.Append(String.Format("<td>{0}</td>", builder.ToString(TagRenderMode.Normal)));
            }

            return new MvcHtmlString(sb.ToString());
        }
    }
}