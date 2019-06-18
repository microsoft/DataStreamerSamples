'use strict'

const { ValueSet } = require('@nodert-win10-rs4/windows.foundation.collections');
const { PropertyValue } = require('@nodert-win10-rs4/windows.foundation');
const { IPropertyValue } = require('@nodert-win10-rs4/windows.foundation');
const { AppServiceConnection } = require('@nodert-win10-rs4/windows.applicationmodel.appservice');
const { AppServiceConnectionStatus } = require('@nodert-win10-rs4/windows.applicationmodel.appservice');
const { AppServiceResponseStatus } = require('@nodert-win10-rs4/windows.applicationmodel.appservice');
const { Package } = require('@nodert-win10-rs4/windows.applicationmodel');

var clientConnected = false;
var excelConnected = false;
var appServiceConnection = null;
var iCnt=0;

const requestReceivedHandler = (sender, args) => {
    var messageDeferral = args.getDeferral();
    var f = args.request.message.first();
    while (f.hasCurrent) {
        var ipvt = IPropertyValue.castFrom(f.current.value);
        var ipvts = ipvt.getString();
        switch (f.current.key) {
            case "Data":
            UpdateConnectionStatus(ipvts);
            break;
            case "ReadData":
            ReportResults("ReadData result: " + ipvts);
            break;
        }
        console.log('DataStreamer::Send response', ipvts);
        f.moveNext();
    }
    messageDeferral.complete();
}

const serviceClosedHandler = (sender, reason) => {
    CloseConnection();
}


function Connect() {
    if (document.readyState === 'complete') {
        document.querySelector('#txtResults').value = "";
        if (appServiceConnection === null) {
            appServiceConnection = new AppServiceConnection();
        }
        appServiceConnection.appServiceName = "com.microsoft.datastreamerconnect";
        appServiceConnection.on("RequestReceived", requestReceivedHandler);
        appServiceConnection.on("ServiceClosed", serviceClosedHandler);

        // Running in a packaging project
         var pfn = Package.current.id.familyName;
        //var pfn = "27cd71fb-8fcd-4cbe-9019-f07ec581365d_n3sawgb4qe5x4";  // for debuging App Service by itself
        appServiceConnection.packageFamilyName = pfn;
        appServiceConnection.openAsync((error, result) => {
            if (error) {
                console.error(error);
                ReportResults("openAsync error: " + result);
                return;
            }

            if (result !== AppServiceConnectionStatus.success) {
                console.error("Failed to connect.");
                ReportResults("openAsync error: " + AppServiceConnectionStatusString(result));
                return;
            }
            ReportResults("openAsync: " + "AppServiceConnectionStatus.success");
            Send();
            AppServiceStatus();
        }) // openAync
    }
}

function Send() {
    // Call the service.

    var message = new ValueSet();
    message.insert("Command", PropertyValue.createString("Connect"));
    message.insert("Role", PropertyValue.createString("DataStreamerConnect"));

    appServiceConnection.sendMessageAsync(message, (error, response) => {
        ReportResults("sendMessageAsync: " + AppServiceConnectionStatusString(response.status));
        if (response.status === AppServiceResponseStatus.success) {
            var f = response.message.first();
            if (f.hasCurrent) {
                var ipvt = IPropertyValue.castFrom(f.current.value);
                var ipvts = ipvt.getString();
                console.log('DataStreamer::Send response', ipvts);
                ReportResults("Send result: " + ipvts);
            }
        }
    });
}

// Write 1000 rows
async function bulkWrite()
{
    for (let i=0;i<1000;i++) {
        await Write(i);
        ReportResults("Write record: " + (i + 1));
        sleep(50);
    }
}

function sleep(milliseconds) {
    var start = new Date().getTime(); for (var i = 0; i < 1e7; i++) { 
        if ((new Date().getTime() - start) > milliseconds) { 
            break;
        } 
    } 
}
function Write(i) {
    return new Promise(function(resolve) {
    var message = new ValueSet();
    message.insert("Command", PropertyValue.createString("Write"));
    message.insert("Data", PropertyValue.createString((i + 1) + "," + Math.floor((Math.random() * 100) + 1) + "," + Math.floor((Math.random() * 100) + 1) + "," + Math.floor((Math.random() * 100) + 1) + "," + Math.floor((Math.random() * 100) + 1) ));

    appServiceConnection.sendMessageAsync(message, (error, response) => {
        ReportResults("sendMessageAsync: " + AppServiceResponseStatusString(response.status));
        if (response.status === AppServiceResponseStatus.success) {
            var f = response.message.first();
            if (f.hasCurrent) {
                var ipvt = IPropertyValue.castFrom(f.current.value);
                var ipvts = ipvt.getString();
                console.log('DataStreamer::Send response', ipvts);
                ReportResults("Write result: " + ipvts);
            }
        }
        resolve();
    });
});
}

function WriteOne() {
    Write(Math.floor(Math.random()*100)+1);
}

function Read() {
    var message = new ValueSet();
    message.insert("Command", PropertyValue.createString("Read"));

    appServiceConnection.sendMessageAsync(message, (error, response) => {
        ReportResults("Read::sendMessageAsync: " + AppServiceResponseStatusString(response.status));
        if (response.status === AppServiceResponseStatus.success) {
            var f = response.message.first();
            while (f.hasCurrent) {
                var ipvt = IPropertyValue.castFrom(f.current.value);
                var ipvts = ipvt.getString();
                console.log('DataStreamer::Send response', ipvts);
                ReportResults("Read result: " + ipvts);
                f.moveNext();
            }
        }
    });

}

function CloseConnection() {
    appServiceConnection.close();
    clientConnected = false;
    excelConnected = false;
    appServiceConnection = null;
    UpdateConnectionStatusStr();
}

function AppServiceStatus() {
    var message = new ValueSet();
    message.insert("Command", PropertyValue.createString("Status"));
    appServiceConnection.sendMessageAsync(message, (error, response) => {
        ReportResults("AppServiceStatus::sendMessageAsync: " + AppServiceResponseStatusString(response.status));
        if (response.status === AppServiceResponseStatus.success) {
            var f = response.message.first();
            while (f.hasCurrent) {
                var ipvt = IPropertyValue.castFrom(f.current.value);
                var ipvts = ipvt.getString();
                if (f.current.key == "Data") {
                    UpdateConnectionStatus(ipvts);
                }
                console.log('DataStreamer::Send response', ipvts);
                ReportResults("AppServiceStatus result: " + ipvts);
                f.moveNext();
            }
        }
    });
}

function AppServiceResponseStatusString(iVal) {
    switch (iVal) {
        case (AppServiceResponseStatus.success):
            return "AppServiceResponseStatus.success";
        case (AppServiceResponseStatus.failure):
            return "AppServiceResponseStatus.failure";
        case (AppServiceResponseStatus.resourceLimitsExceeded):
            return "AppServiceResponseStatus.resourceLimitsExceeded";
        case (AppServiceResponseStatus.unknown):
            return "AppServiceResponseStatus.unknown";
        case (AppServiceResponseStatus.remoteSystemUnavailable):
            return "AppServiceResponseStatus.remoteSystemUnavailable";
        case (AppServiceResponseStatus.messageSizeTooLarge):
            return "AppServiceResponseStatus.messageSizeTooLarge";
        default:
            return "";
    }
}


function AppServiceConnectionStatusString(iVal) {
    switch (iVal) {
        case (AppServiceConnectionStatus.success):
            return "AppServiceConnectionStatus.success";
        case (AppServiceConnectionStatus.appNotInstalled):
            return "AppServiceConnectionStatus.appNotInstalled";
        case (AppServiceConnectionStatus.appUnavailable):
            return "AppServiceConnectionStatus.appUnavailable";
        case (AppServiceConnectionStatus.appServiceUnavailable):
            return "AppServiceConnectionStatus.appServiceUnavailable";
        case (AppServiceConnectionStatus.unknown):
            return "AppServiceConnectionStatus.unknown";
        case (AppServiceConnectionStatus.remoteSystemUnavailable):
            return "AppServiceConnectionStatus.remoteSystemUnavailable";
        case (AppServiceConnectionStatus.remoteSystemNotSupportedByApp):
            return "AppServiceConnectionStatus.remoteSystemNotSupportedByApp";
        case (AppServiceConnectionStatus.notAuthorized):
            return "AppServiceConnectionStatus.notAuthorized";
        default:
            return "";
    }
}

function ReportResults(strResult) {
    // document.querySelector('#txtResults').value += "\n" + strResult;
    var res = document.querySelector('#txtResults').value;
    document.querySelector('#txtResults').value = strResult + "\n" + res;

}

function UpdateConnectionStatusStr() {
    UpdateConnectionStatus("Client:" + clientConnected + ",Excel:" + excelConnected);
}
function UpdateConnectionStatus(str) {
    //"Client:False,Excel:True"
    var res = str.split(",");
    var client = res[0].split(":");
    var excel = res[1].split(":");
    if (client[1].toUpperCase() == "TRUE") {
        document.querySelector('#cbClient').checked = true;
        document.querySelector('#btnClose').disabled = false;
        document.querySelector('#btnClose').className = "buttonEnabled";
        document.querySelector('#btnConnect').disabled = true;
        document.querySelector('#btnConnect').className = "buttonDisabled";
        clientConnected = true;
    } else {
        document.querySelector('#cbClient').checked = false;
        document.querySelector('#btnClose').disabled = true;
        document.querySelector('#btnClose').className = "buttonDisabled";
        document.querySelector('#btnConnect').disabled = false;
        document.querySelector('#btnConnect').className = "buttonEnabled";
        clientConnected = false;
    }
    if (excel[1].toUpperCase() == "TRUE") {
        document.querySelector('#cbExcel').checked = true;
        excelConnected = true;
        document.querySelector('#btnWrite').disabled = false;
        document.querySelector('#btnWrite').className = "buttonEnabled";
        document.querySelector('#btnBulkWrite').disabled = false;
        document.querySelector('#btnBulkWrite').className = "buttonEnabled";
        document.querySelector('#btnRead').disabled = false;
        document.querySelector('#btnRead').className = "buttonEnabled";
    } else {
        document.querySelector('#cbExcel').checked = false;
        excelConnected = false;
        document.querySelector('#btnWrite').disabled = true;
        document.querySelector('#btnWrite').className = "buttonDisabled";
        document.querySelector('#btnBulkWrite').disabled = true;
        document.querySelector('#btnBulkWrite').className = "buttonDisabled";
        document.querySelector('#btnRead').disabled = true;
        document.querySelector('#btnRead').className = "buttonDisabled";
    }
}

document.onreadystatechange = () => {
    document.querySelector('#btnConnect').addEventListener('click', Connect);
    document.querySelector('#btnWrite').addEventListener('click', WriteOne);
    document.querySelector('#btnBulkWrite').addEventListener('click', bulkWrite);
    document.querySelector('#btnRead').addEventListener('click', Read);
    document.querySelector('#btnClose').addEventListener('click', CloseConnection);
    Connect();
}

