import { Injectable } from "@angular/core";
import * as signalR from "@aspnet/signalr";
import { environment } from "../../environments/environment";
import { HubConnection } from "@aspnet/signalr";
import { AppStateService } from "./app-state.service";

@Injectable({
    providedIn: "root"
})
export class SignalrService {
    connection: any;

    connectionClosedByUser: boolean = false;

    constructor(private appState: AppStateService) {}

    public createConnection(groupName: string, hubUrl: string) {
        if (
            !this.connection ||
            this.connection.connection.connectionState === 2
        ) {
            this.connect(
                groupName,
                hubUrl
            ).then(data => {
                console.log(`SignalR hub ${hubUrl} connected.`);
                if (
                    this.connection.connection.connectionState === 1 &&
                    !this.connectionClosedByUser
                ) {
                    console.log(`Connecting to group ${groupName}`);
                    this.connection.send("joinGroup", groupName);
                }
            });
        } else {
            if (this.connection.connection.connectionState === 1) {
                console.log(`Connecting to group ${groupName}`);
                this.connection.send("joinGroup", groupName);
            }
        }
    }

    public closeConnection(groupName: string) {
        this.connectionClosedByUser = true;
        if (
            this.connection &&
            this.connection.connection.connectionState === 1
        ) {
            console.log(`Disconnecting from group ${groupName}`);
            this.connection.send("leaveGroup", groupName);
            //   console.log(`Stoping SignalR connection`);
            //   this.connection.stop();
        }
    }

    public validateResponse(responce: any): boolean {
        debugger;
        if (
            responce.ids &&
            responce.ids.length > 0 &&
            responce.senderId &&
            responce.senderId !== this.appState.currentDatabaseUser.id
        ) {
            return true;
        } else {
            return false;
        }
    }
    connect(groupName: string, hubUrl: string): Promise<void> {
        if (!this.connection) {
            console.log(
                `SignalR hub ${hubUrl} connection is corrupted.Creating new one...`
            );
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${environment.apiUrl}/${hubUrl}`)
                .build();

            this.connection.onclose(err => {
                console.log(`SignalR hub ${hubUrl} disconnected.`);
                if (this.connectionClosedByUser) {
                    this.createConnection(groupName, hubUrl);
                }
            });
        }
        console.log(`SignalR hub ${hubUrl} reconnection started...`);
        return this.connection
            .start()
            .catch(err => console.log("SignalR ERROR " + err));
    }
}
