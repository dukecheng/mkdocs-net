using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.AspNet.WebApis.Filters;
using AgileLabs.Json;
using AgileLabs.Json.Converters;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Niusys.Docs.Web.ServiceRegisters
{
    public class SwaggerRegister : IServiceRegister, IRequestPiplineRegister, IMvcBuildConfig, IMvcOptionsConfig
    {
        public int Order => 0;

        public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
        {
            piplineActions.Register("swagger", RequestPiplineStage.BeforeRouting, app =>
            {
                // Register the Swagger generator and the Swagger UI middlewares
                app.UseOpenApi();
                app.UseSwaggerUi();
            });
            return piplineActions;
        }

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            // Register the Swagger services
            //services.AddSwaggerDocument();
            services.AddOpenApiDocument(options =>
            {
                options.Title = $"MKDocs Apis";
                options.DocumentName = "all";
                var mvcJsonSetting = new System.Text.Json.JsonSerializerOptions()
                {
                    PropertyNamingPolicy = LowerUnderlineCaseJsonNamingPolicy.Instance
                };
                //options.ApplySettings(, mvcJsonSetting);
                //JsonNetSerializerSettings.Instance
            });
        }

        private static void ConfigureMvcJsonSerlization(IMvcBuilder mvcBuilder)
        {

        }

        public void ConfigureMvcBuilder(IMvcBuilder mvcBuilder, AppBuildContext appBuildContext)
        {
            JsonNetSerializerSettings.Instance = JsonNetSerializerSettings.CamelCase;
            // Newtonsoft.Json
            mvcBuilder.AddNewtonsoftJson(jsonOptions =>
            {
                JsonNetSerializerSettings.DeconretCamelCaseSerializerSettings(jsonOptions.SerializerSettings);
                //jsonOptions.SerializerSettings.Converters.Add(CustBoolIntJsonConverter.Instance);
                jsonOptions.SerializerSettings.Converters.Add(DateTimeToTimestampJsonConverter.Instance);
                //jsonOptions.SerializerSettings.Converters.Add(StringArrayToStringJsonConverter.Instance);
                jsonOptions.SerializerSettings.Converters.Add(NullableDateTimeToTimestampJsonConverter.Instance);
                jsonOptions.SerializerSettings.Converters.Add(LongToStringJsonConverter.Instance);
                //jsonOptions.SerializerSettings.Converters.Add(AntdFilterStringArrayJsonConverter.Instance);
            });
        }

        public void ConfigureMvcOptions(MvcOptions mvcOptions, AppBuildContext appBuildContext)
        {
            mvcOptions.Filters.Add<ExceptionHandlerFilter>(99);
            mvcOptions.Filters.Add<EnvelopFilterAttribute>(98);
            mvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        }
        public class LongToStringJsonConverter : JsonConverter<long>
        {
            public static LongToStringJsonConverter Instance { get; private set; }

            static LongToStringJsonConverter()
            {
                Instance = new LongToStringJsonConverter();
            }

            public override long ReadJson(JsonReader reader, Type objectType, [AllowNull] long existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (long.TryParse(reader.Value?.ToString(), out var realValue))
                {
                    return realValue;
                }

                return existingValue;
            }

            public override void WriteJson(JsonWriter writer, [AllowNull] long value, JsonSerializer serializer)
            {
                if (value > 0x20000000000000)
                {
                    writer.WriteValue(value.ToString());
                }
                else
                {
                    writer.WriteValue(value);
                }
            }
        }
        public class LowerUnderlineCaseJsonNamingPolicy : System.Text.Json.JsonNamingPolicy
        {
            public static System.Text.Json.JsonNamingPolicy Instance { get; }

            private Regex _regex = new Regex("(?!(^[A-Z]))([A-Z])");

            static LowerUnderlineCaseJsonNamingPolicy()
            {
                Instance = new LowerUnderlineCaseJsonNamingPolicy();
            }

            public override string ConvertName(string propertyName)
            {
                return _regex.Replace(propertyName, "_$2").ToLower();
            }
        }
    }

}
