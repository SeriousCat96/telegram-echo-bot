import { Component, OnDestroy } from '@angular/core';
import { Message } from './models/message.model';
import { NotificationType } from './models/message.notificationType.model';
import { MessagesSignalRService } from './services/messages.signalR.service';
import { BehaviorSubject } from 'rxjs';

@Component({
    selector: 'botsweb-app',
    templateUrl: './app.component.html',
})
export class AppComponent implements OnDestroy { 
    constructor(
        private signalRService: MessagesSignalRService
      )
    {
        signalRService.startConnection();

        signalRService.addEventListener(
            NotificationType.Message,
            (message: Message) => {

                console.log('[Message]', message.text);
                if (!this.messagesSubject.value.find(m => m.id == message.id))
                {
                    this.messagesSubject.next([message, ...this.messagesSubject.value]);
                }
            }
        );
    }

    messagesSubject = new BehaviorSubject<Message[]>([]);

    get items() {
        return this.messagesSubject.value;
    }

    ngOnDestroy(): void {
        this.signalRService.stopConnection();
    }

    public startConnection(): void {
        this.signalRService
            .startConnection()
            .catch(err => console.log(err));
    }
}