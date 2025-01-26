using EmbedIO;
using LeagueProxyLib;
using System.Text.Json;
using System.Text.Json.Nodes;

class App
{
    public static async Task Main(string[] args)
    {
        bool strict = args.Contains("--strict");
        bool norestart = args.Contains("--norestart");

        if (strict)
        {
            Console.WriteLine("Older League Client version without embedded Vanguard will be restored!");
        }
        if (norestart)
        {
            Console.WriteLine("Riot Client will not prompt you to restart your PC now!");
        }
        if (!norestart)
        {
            Console.WriteLine("Vanguard errors supressed and enforcement disabled!");
        }

        var leagueProxy = new LeagueProxy();

        leagueProxy.Events.HookClientConfigPublic += (string content, IHttpRequest request) =>
        {
            var configObject = JsonSerializer.Deserialize<JsonNode>(content);

            if (!norestart)
            {
                //disable vanguard and supress errors
                SetKey(configObject, "anticheat.vanguard.backgroundInstall", false);
                SetKey(configObject, "anticheat.vanguard.enabled", false);
                SetKey(configObject, "keystone.client.feature_flags.vanguardLaunch.disabled", true);
                SetKey(configObject, "lol.client_settings.vanguard.enabled", false);
                SetKey(configObject, "lol.client_settings.vanguard.enabled_embedded", false);
                SetKey(configObject, "lol.client_settings.vanguard.url", "");
                RemoveVanguardDependencies(configObject, "keystone.products.league_of_legends.patchlines.live");
                RemoveVanguardDependencies(configObject, "keystone.products.league_of_legends.patchlines.pbe");
                RemoveVanguardDependencies(configObject, "keystone.products.valorant.patchlines.live");
            }

            if (norestart)
            {
                SetKey(configObject, "keystone.client.feature_flags.pcbang_vanguard_restart_bypass.disabled", true); 
                SetKey(configObject, "keystone.client.feature_flags.restart_required.disabled", true);
            }

            if (strict)
            {
                RemoveMvgModuleMac(configObject, "keystone.products.league_of_legends.patchlines.live"); //restores old client version without vg module
            }

            //since were already hooked into clientconfig, why not disable client telemtry and do some minor optimizations?
            SetKey(configObject, "keystone.client.feature_flags.self_update_in_background.enabled", false);
            SetKey(configObject, "keystone.client_config.diagnostics_enabled", false);
            SetKey(configObject, "keystone.telemetry.heartbeat_custom_metrics", false);
            SetKey(configObject, "keystone.client.feature_flags.dismissible_name_change_modal.enabled", true); //lets you optionally ignore forced name change
            SetKey(configObject, "keystone.riotgamesapi.telemetry.heartbeat_products", false);
            SetKey(configObject, "keystone.client.feature_flags.mfa_notification.enabled", false); // hides enable 2fa nag
            SetKey(configObject, "keystone.client.feature_flags.autoPatch.disabled", true); // hides enable auto update nag
            SetKey(configObject, "keystone.riotgamesapi.telemetry.heartbeat_voice_chat_metrics", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.newrelic_events_v2_enabled", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.newrelic_metrics_v1_enabled", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.newrelic_schemaless_events_v2_enabled", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.opentelemetry_events_enabled", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.opentelemetry_uri_events", "");
            SetKey(configObject, "keystone.riotgamesapi.telemetry.singular_v1_enabled", false);
            SetKey(configObject, "riot.eula.agreementBaseURI", ""); // bypasses forced ToS acceptance popup
            SetKey(configObject, "keystone.telemetry.heartbeat_products", false);
            SetKey(configObject, "keystone.telemetry.heartbeat_voice_chat_metrics", false);
            SetKey(configObject, "keystone.telemetry.send_error_telemetry_metrics", false);
            SetKey(configObject, "keystone.telemetry.send_product_session_start_metrics", false);
            SetKey(configObject, "keystone.telemetry.singular_v1_enabled", false);
            SetKey(configObject, "lol.client_settings.startup.should_wait_for_home_hubs", false); // fixes home hubs loading issue
            SetKey(configObject, "lol.client_settings.startup.should_show_progress_bar_text", false);
            SetKey(configObject, "lol.client_settings.remedy.is_verbal_abuse_remedy_modal_enabled", false); // hides the cringe were sorry you had a bad time modal after you report someone
            SetKey(configObject, "lol.game_client_settings.app_config.singular_enabled", false);
            SetKey(configObject, "lol.game_client_settings.low_memory_reporting_enabled", false);
            SetKey(configObject, "lol.game_client_settings.telemetry.standalone.long_frame_cooldown", 999);
            SetKey(configObject, "lol.game_client_settings.telemetry.standalone.long_frame_min_time", 99999);
            SetKey(configObject, "lol.game_client_settings.telemetry.standalone.nr_sample_rate", 0);
            SetKey(configObject, "lol.game_client_settings.telemetry.standalone.sample_rate", 0);
            SetKey(configObject, "lol.client_settings.display_legacy_patch_numbers", true); // shows old patch format in client (eg. 15.2 instead of 25.S2.2)
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "applicationID", "");
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "clientToken", "");
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "isEnabled", false); // nukes datadog telemetry service
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "service", "");
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "sessionReplaySampleRate", 0);
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "sessionSampleRate", 0);
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "site", "");
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "telemetrySampleRate", 0);
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "traceSampleRate", 0);
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "trackLongTasks", false);
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "trackResources", false);
            SetNestedKeys(configObject, "lol.client_settings.datadog_rum_config", "trackUserInteractions", false);
            SetNestedKeys(configObject, "lol.client_settings.sentry_config", "isEnabled", false);// nukes sentry telemetry service
            SetNestedKeys(configObject, "lol.client_settings.sentry_config", "sampleRate", 0);
            SetNestedKeys(configObject, "lol.client_settings.sentry_config", "dsn", "");

            return JsonSerializer.Serialize(configObject);
        };

        leagueProxy.Events.HookClientConfigPlayer += (string content, IHttpRequest request) =>
        {
            var configObject = JsonSerializer.Deserialize<JsonNode>(content);

            SetKey(configObject, "keystone.riotgamesapi.telemetry.endpoint.send_deprecated", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.endpoint.send_failure", false);
            SetKey(configObject, "keystone.riotgamesapi.telemetry.endpoint.send_success", false);
            SetKey(configObject, "keystone.telemetry.metrics_enabled", false);
            SetKey(configObject, "keystone.telemetry.newrelic_events_v2_enabled", false);
            SetKey(configObject, "keystone.telemetry.newrelic_metrics_v1_enabled", false);
            SetKey(configObject, "lol.client_settings.player_behavior.display_v1_ban_notifications", true); // fixes unknown player bug where ban reason is not shown
            SetKey(configObject, "lol.game_client_settings.logging.enable_http_public_logs", false);
            SetKey(configObject, "lol.game_client_settings.logging.enable_rms_public_logs", false);
            SetKey(configObject, "keystone.telemetry.newrelic_schemaless_events_v2_enabled", false);
            SetKey(configObject, "lol.client_settings.metrics.enabled", false);
            SetNestedKeys(configObject, "lol.client_settings.deepLinks", "launchLorEnabled", false); // hides lor button
            SetEmptyArrayForConfig(configObject, "chat.xmpp_stanza_response_telemetry_allowed_codes");
            SetEmptyArrayForConfig(configObject, "chat.xmpp_stanza_response_telemetry_allowed_iqids");

            return JsonSerializer.Serialize(configObject);
        };

            var process = leagueProxy.StartAndLaunchRCS(args);
        if (process is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to start Riot Client. Please open issue on github if this persist");
            Console.ResetColor();
            leagueProxy.Stop();
            return;
        }
        await process.WaitForExitAsync();
        leagueProxy.Stop();
    }

    static void RemoveMvgModuleMac(JsonNode? configObject, string patchline)
    {
        if (configObject == null) return;

        var productNode = configObject[patchline];
        if (productNode is not null)
        {
            var configs = productNode["platforms"]?["mac"]?["configurations"]?.AsArray();
            if (configs != null)
            {
                foreach (var config in configs)
                {
                    if (config?["patch_url"] is not null)
                    {
                        config["patch_url"] = "https://lol.secure.dyn.riotcdn.net/channels/public/releases/CF8CCD333558383E.manifest";
                    }

                    var patchArtifacts = config?["patch_artifacts"]?.AsArray();
                    if (patchArtifacts != null)
                    {
                        foreach (var artifact in patchArtifacts)
                        {
                            if (artifact?["type"]?.ToString() == "patch_url")
                            {
                                artifact["patch_url"] = "https://lol.secure.dyn.riotcdn.net/channels/public/releases/CF8CCD333558383E.manifest";
                            }
                        }
                    }

                    if (config != null)
                    {
                        config["launchable_on_update_fail"] = true;
                    }
                }
            }
        }
    }
    static void RemoveVanguardDependencies(JsonNode? configObject, string path)
    {
        if (configObject == null) return;

        var productNode = configObject[path];
        if (productNode is not null)
        {
            var configs = productNode["platforms"]?["win"]?["configurations"]?.AsArray();
            if (configs is not null)
            {
                foreach (var config in configs)
                {
                    var dependencies = config?["dependencies"]?.AsArray();
                    var vanguard = dependencies?.FirstOrDefault(x => x!["id"]!.GetValue<string>() == "vanguard");
                    if (vanguard is not null)
                    {
                        dependencies?.Remove(vanguard);
                    }
                }
            }
        }
    }
    private static void SetEmptyArrayForConfig(JsonNode? configObject, string configKey)
    {
        if (configObject?[configKey] is JsonArray)
        {
            configObject[configKey] = new JsonArray();
        }
        else if (configObject?[configKey] is JsonObject jsonObject)
        {
            jsonObject[configKey] = new JsonArray();
        }
    }
    private static void SetNestedKeys(JsonNode? configObject, string parentKey, string childKey, object value)
    {
        if (configObject == null) return;
        if (configObject?[parentKey] is JsonNode parentNode)
        {
            parentNode[childKey] = value switch
            {
                bool boolValue => (JsonNode)boolValue,
                string stringValue => (JsonNode)stringValue,
                double doubleValue => (JsonNode)doubleValue,
                int intValue => (JsonNode)intValue,
                _ => throw new InvalidOperationException($"Unsupported type: {value.GetType()}"),
            };
        }
    }
    static void SetKey(JsonNode? configObject, string key, object value)
    {
        if (configObject == null) return;

        if (configObject[key] != null)
        {
            configObject[key] = value switch
            {
                bool boolValue => (JsonNode)boolValue,
                int intValue => (JsonNode)intValue,
                double doubleValue => (JsonNode)doubleValue,
                string stringValue => (JsonNode)stringValue,
                _ => throw new InvalidOperationException($"Unsupported type: {value.GetType()}"),
            };
        }
    }
}