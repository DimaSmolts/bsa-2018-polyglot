import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpService, RequestMethod } from '../../services/http.service';
import { ngfModule, ngf } from 'angular-file';
import { ProjectService } from '../../services/project.service';
import { SnotifyService, SnotifyPosition, SnotifyToastConfig } from 'ng-snotify';

@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.sass']
})
export class UploadFileComponent implements OnInit {

  @Output() fileEvent = new EventEmitter<File>();
  fileToUpload: File;
  formData: FormData;

  validDrag;
  invalidDrag;


  constructor(private service: ProjectService, private snotifyService: SnotifyService) {
    this.formData = new FormData();
   }

  ngOnInit() {
  }


  ConfirmFile($event) {
    debugger;
    this.fileEvent.emit($event);
    this.fileToUpload = $event[$event.length - 1];      
  }

  Upload() {
    this.formData.append("file", this.fileToUpload);
    let projectId = 1;
    this.service.postFile(projectId, this.formData)
    .subscribe(
      (d) => { 
          this.snotifyService.success("File Uploaded", "Success!");
        },
        err => {
          this.snotifyService.error("File wasn`t uploaded", "Error!");
        }
      );
  }

}
