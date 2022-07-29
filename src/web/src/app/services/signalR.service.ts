import { HubConnection, HubConnectionBuilder, HttpTransportType, LogLevel } from "@microsoft/signalr";
import { environment } from '../../environments/environment';

const API_URL = `${environment.apiUrl}`;

export abstract class SignalRService {
    private hubConnection: HubConnection;

    constructor(
        private url: string,
        private transport: HttpTransportType = HttpTransportType.None,
        private logLevel: LogLevel = LogLevel.Warning,
    ) {
    }

    /**
     * Открывает подключение к серверу.
     */
    public startConnection(): Promise<void> {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl(`${API_URL}${this.url}`, { transport: this.transport })
            .withAutomaticReconnect()
            .configureLogging(this.logLevel)
            .build();

        return this.hubConnection.start();
    }

    /**
     * Закрывает подключение к серверу.
     */
    public stopConnection(): Promise<void> {
        return this.hubConnection.stop();
    }

    /**
     * Подписаться на события с сервера.
     * @param method Название метода сервера, для которого необходимо получать данные.
     * @param callback Callback, который вызывается при получении данных с сервера.
     */
    public addEventListener(method: string, callback: (...args: any[]) => void): void {
        this.hubConnection.on(method, callback);
    }

    /**
     * Отправить данные на сервер.
     * @param method Название метода сервера, который необходимо вызывать для обработки данных.
     * @param args Данные, которые передаются на сервер.
     */
    public sendData(method: string, ...args: any[]): void {
        this.hubConnection.send(method, args);
    }
}