﻿// See https://aka.ms/new-console-template for more information
#pragma warning disable IDE1006

using ddns;

using Newtonsoft.Json;

using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

string TENCENT_CLOUD_API_HOST = "dnspod.tencentcloudapi.com";
string DOMAIN_RECORD_TYPE = "A";
string DOMAIN_RECORD_LINE = "默认";

DnspodClient CreateDnspodClient() {
    return new DnspodClient(new Credential {
        SecretId = Config.Id,
        SecretKey = Config.Key,
    }, "", new ClientProfile {
        HttpProfile = new HttpProfile {
            Endpoint = TENCENT_CLOUD_API_HOST,
        }
    });
}

try {
    foreach (var item in CreateDnspodClient().DescribeRecordListSync(new DescribeRecordListRequest {
        Domain = Config.Domain
    }).RecordList) {
        if (item.Name.Equals(Config.SubDomain)) {
            if (ipify.GetIP() is string ip) {
                if (CreateDnspodClient().ModifyRecordSync(new ModifyRecordRequest {
                    Domain = Config.Domain,
                    SubDomain = item.Name,
                    RecordType = DOMAIN_RECORD_TYPE,
                    RecordLine = DOMAIN_RECORD_LINE,
                    Value = ip,
                    RecordId = item.RecordId,
                }).RequestId is string) {
                    Console.WriteLine($"{Config.SubDomain}.{Config.Domain} → {ip}");
                }
            }
            break;
        }
    }
} catch (Exception e) {
    Console.WriteLine(e.ToString());
}

Thread.Sleep(1000);

class ipify {
#pragma warning disable CS0649
    public string? ip = null;
#pragma warning restore CS0649

    public static string? GetIP() {
        var response = new HttpClient().GetAsync("https://api.ipify.org/?format=json").Result;
        var body = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<ipify>(body)?.ip;
    }
}
