import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ComplexStringService } from "../../../services/complex-string.service";
import { MatDialog } from "@angular/material";
import { ImgDialogComponent } from "../../../dialogs/img-dialog/img-dialog.component";
import { ConfirmDialogComponent } from "../../../dialogs/confirm-dialog/confirm-dialog.component";
import { SnotifyService } from "ng-snotify";
import { StringDialogComponent } from "../../../dialogs/string-dialog/string-dialog.component";
import { Tag } from "../../../models";

@Component({
    selector: "app-workspace-key",
    templateUrl: "./key.component.html",
    styleUrls: ["./key.component.sass"]
})
export class KeyComponent implements OnInit {
    @Input()
    public key: any;
    @Input()
    public tags: string[];
    @Output()
    removeEvent = new EventEmitter<number>();
    @Output()
    editKey = new EventEmitter<number>();
    description: string = "Are you sure you want to remove the string?";
    btnOkText: string = "Delete";
    btnCancelText: string = "Cancel";
    answer: boolean;

    constructor(
        private dataProvider: ComplexStringService,
        public dialog: MatDialog,
        private snotifyService: SnotifyService
    ) {}

    ngOnInit() {}

    onDeleteString() {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: "500px",
            data: {
                description: this.description,
                btnOkText: this.btnOkText,
                btnCancelText: this.btnCancelText,
                answer: this.answer
            }
        });
        dialogRef.afterClosed().subscribe(result => {
            if (dialogRef.componentInstance.data.answer) {
                this.dataProvider.delete(this.key.id).subscribe(
                    response => {
                        if (response) {
                            this.snotifyService.success(
                                "String deleted",
                                "Success!"
                            );
                            this.removeEvent.emit(this.key.id);
                        } else {
                            this.snotifyService.error(
                                "String wasn`t deleted",
                                "Error!"
                            );
                        }
                    },
                    err => {
                        this.snotifyService.error(
                            "String wasn`t deleted",
                            "Error!"
                        );
                    }
                );
            }
        });
    }

    onEditStringClick() {
		this.dialog.open(StringDialogComponent, {
				data: {
                    projectId: this.key.projectId,
                    string: this.key,
                    tags: this.tags
                    
                    
				}
		}).afterClosed().subscribe(() => {
			this.dataProvider.getById(this.key.id).subscribe(data => {
				this.key = data;
				this.snotifyService.success("String edited", "Success!");
			}, 
			err => {
				this.snotifyService.error("String wasn`t edited", "Error!");
			});
			this.editKey.emit(this.key.id);
		});
	}

    onPictureIconClick(key: any) {
        let dialogRef = this.dialog.open(ImgDialogComponent, {
            data: {
                imageUri: key.pictureLink
            }
        });
    }
}
