import { Injectable } from '@angular/core';
import { HttpTransportType, LogLevel } from "@microsoft/signalr";
import { SignalRService } from './signalR.service';

@Injectable({
    providedIn: 'root'
})
export class MessagesSignalRService extends SignalRService {

    constructor() {
        super('/hubs/messages', HttpTransportType.None, LogLevel.Information);
    }
}