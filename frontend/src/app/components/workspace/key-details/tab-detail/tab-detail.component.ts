import { Component, OnInit, Input } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ImgDialogComponent } from '../../../../dialogs/img-dialog/img-dialog.component';
import { IString } from '../../../../models/string';

@Component({
  selector: 'app-tab-detail',
  templateUrl: './tab-detail.component.html',
  styleUrls: ['./tab-detail.component.sass']
})
export class TabDetailComponent implements OnInit {

  @Input()  public keyDetails: IString;

  constructor(public dialog: MatDialog) { }

  ngOnInit() {
  }

  onImageClick(keyDetails){
    if(keyDetails.pictureLink){
    let dialogRef = this.dialog.open(ImgDialogComponent, {
      data: {
        imageUri: keyDetails.pictureLink
      }
      });
    }
  }

}
