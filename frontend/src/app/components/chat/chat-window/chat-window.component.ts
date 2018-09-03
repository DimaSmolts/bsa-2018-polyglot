import {
    Component,
    OnInit,
    ViewChild,
    ElementRef,
    Renderer2,
    Input,
    SimpleChanges
} from "@angular/core";
import { ProjectService } from "../../../services/project.service";
import { MatSnackBar } from "@angular/material";
import { ChatService } from "../../../services/chat.service";
import { GroupType } from "../../../models";
import { SignalrService } from "../../../services/signalr.service";

@Component({
    selector: "app-chat-window",
    templateUrl: "./chat-window.component.html",
    styleUrls: ["./chat-window.component.sass"]
})
export class ChatWindowComponent implements OnInit {
    @ViewChild("mainwindow")
    mainWindow: ElementRef;
    @Input()
    interlocutor: any;
    public currentMessage: string = "";

    messages = [];
    constructor(
        private renderer: Renderer2,
        private chatService: ChatService,
        public snackBar: MatSnackBar,
        private signalRService: SignalrService
    ) {}

    ngOnInit() {
        //  this.messages = MOCK_MESSAGES;
        debugger;
    }

    ngOnDestroy() {
        debugger;
    }

    ngOnChanges(changes: SimpleChanges) {
        this.getMessagesHistory();
    }

    ngAfterViewInit() {
        let scrollHeight = this.mainWindow.nativeElement.scrollHeight;
        this.renderer.setProperty(
            this.mainWindow.nativeElement,
            "scrollTop",
            scrollHeight
        );
    }

    getMessagesHistory() {
        if (this.interlocutor) {
            this.chatService
                .getMessagesHistory(GroupType.users, this.interlocutor.id)
                .subscribe(messages => {
                    if (messages) {
                        debugger;
                        this.messages = messages;
                    }
                });
        }
    }

    sendMessage() {
        if (this.currentMessage.length > 0) {
            this.messages.push({
                senderId: 1,
                recipientId: this.interlocutor.id,
                body: this.currentMessage,
                receivedDate: Date.now(),
                isRead: false
            });
            this.currentMessage = "";
            this.signalRService.sendMessage("", this.currentMessage);
        }
    }

    toggleSelection(message) {
        this.openSnackBar();
    }

    openSnackBar() {
        this.snackBar.open("mmmmmmmm", "sdfsdf", {
            duration: 2000
        });
    }
}

const MOCK_MESSAGES = [
    {
        body:
            "Do you know the difference between education and experience? Education is what you get when you read the fine print; experience is what you get when you don't",
        date: Date.now(),
        user: {
            fullName: "Julia Louis-Dreyfus",
            avatarUrl:
                "https://www.randomlists.com/img/people/julia_louis_dreyfus.jpg",
            isOnline: false
        }
    },
    {
        body: "No wonder you're tired! You understood so much today. ",
        date: Date.now(),
        user: {
            fullName: "Jennifer Love Hewitt",
            avatarUrl:
                "https://www.randomlists.com/img/people/jennifer_love_hewitt.jpg",
            isOnline: true
        }
    },
    {
        body:
            "My father, a good man, told me, 'Never lose your ignorance; you cannot replace it.'",
        date: Date.now(),
        user: {
            fullName: "Natalya Rudakova",
            avatarUrl:
                "https://www.randomlists.com/img/people/natalya_rudakova.jpg",
            isOnline: true
        }
    },
    {
        body:
            "If truth is beauty, how come no one has their hair done in the library?",
        date: Date.now(),
        user: {
            fullName: "Natalya Rudakova",
            avatarUrl:
                "https://www.randomlists.com/img/people/natalya_rudakova.jpg",
            isOnline: true
        }
    },
    {
        body:
            "Ignorance must certainly be bliss or there wouldn't be so many people so resolutely pursuing it.",
        date: Date.now(),
        user: {
            fullName: "Jennifer Love Hewitt",
            avatarUrl:
                "https://www.randomlists.com/img/people/jennifer_love_hewitt.jpg",
            isOnline: true
        }
    },
    {
        body: "the high school after high school!",
        date: Date.now(),
        user: {
            fullName: "Natalya Rudakova",
            avatarUrl:
                "https://www.randomlists.com/img/people/natalya_rudakova.jpg",
            isOnline: true
        }
    },
    {
        body: "A professor is one who talks in someone else's sleep. ",
        date: Date.now(),
        user: {
            fullName: "Natalya Rudakova",
            avatarUrl:
                "https://www.randomlists.com/img/people/natalya_rudakova.jpg",
            isOnline: true
        }
    },
    {
        body: "Never let your schooling interfere with your education. ",
        date: Date.now(),
        user: {
            fullName: "Jennifer Love Hewitt",
            avatarUrl:
                "https://www.randomlists.com/img/people/jennifer_love_hewitt.jpg",
            isOnline: true
        }
    },
    {
        body:
            "About all some men accomplish in life is to send a son to Harvard. ",
        date: Date.now(),
        user: {
            fullName: "Jennifer Love Hewitt",
            avatarUrl:
                "https://www.randomlists.com/img/people/jennifer_love_hewitt.jpg",
            isOnline: true
        }
    },
    {
        body: " He that teaches himself has a fool for a master",
        date: Date.now(),
        user: {
            fullName: "Hugh Jackman",
            avatarUrl:
                "https://www.randomlists.com/img/people/hugh_jackman.jpg",
            isOnline: false
        }
    },
    {
        body:
            "You may have heard that a dean is to faculty as a hydrant is to a dog",
        date: Date.now(),
        user: {
            fullName: "Natalya Rudakova",
            avatarUrl:
                "https://www.randomlists.com/img/people/natalya_rudakova.jpg",
            isOnline: true
        }
    },
    {
        body:
            "The world is coming to an end! Repent and return those library books!",
        date: Date.now(),
        user: {
            fullName: "Jennifer Love Hewitt",
            avatarUrl:
                "https://www.randomlists.com/img/people/jennifer_love_hewitt.jpg",
            isOnline: true
        }
    },
    {
        body: "This is the sort of English up with which I will not put",
        date: Date.now(),
        user: {
            fullName: "Hugh Jackman",
            avatarUrl:
                "https://www.randomlists.com/img/people/hugh_jackman.jpg",
            isOnline: false
        }
    },
    {
        body:
            "So, is the glass half empty, half full, or just twice as large as it needs to be? ",
        date: Date.now(),
        user: {
            fullName: "Julia Louis-Dreyfus",
            avatarUrl:
                "https://www.randomlists.com/img/people/julia_louis_dreyfus.jpg",
            isOnline: false
        }
    },
    {
        body: "OK, now let's look at four dimensions on the blackboard.",
        date: Date.now(),
        user: {
            fullName: "Hugh Jackman",
            avatarUrl:
                "https://www.randomlists.com/img/people/hugh_jackman.jpg",
            isOnline: false
        }
    },
    {
        body: "Having a wonderful wine, wish you were beer. ",
        date: Date.now(),
        user: {
            fullName: "Theodore Roosevelt",
            avatarUrl:
                "https://www.randomlists.com/img/people/theodore_roosevelt.jpg",
            isOnline: false
        }
    }
];