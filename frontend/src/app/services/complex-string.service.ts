import { Injectable } from '@angular/core';
import { HttpService, RequestMethod } from './http.service';
import { IString } from '../models/string';
import { Translation } from '../models/translation'
import { Observable } from 'rxjs';
import { Comment } from '../models/comment'

@Injectable({
  providedIn: 'root'
})

export class ComplexStringService {
  api: string;
  constructor(private dataService: HttpService) {
    this.api = "complexstrings";
  }

  getAll(): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Get, this.api);
  }

  getById(id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Get, this.api, id);
  }

  create(data: FormData): Observable<IString> {
    return this.dataService.sendRequest(RequestMethod.Post, this.api, '', data, undefined, 'form-data');
  }

  update(data: FormData, id: number): Observable<IString> {
    return this.dataService.sendRequest(RequestMethod.Put, this.api, id, data, undefined, 'form-data');
  }

  getStringTranslations(id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Get, this.api + '/' + id + '/translations');
  }

  createStringTranslation(translation: Translation, id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Post, this.api + '/' + id + '/translations', undefined, translation);
  }

  editStringTranslation(translation: Translation, id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Put, this.api + '/' + id + '/translations', undefined, translation);
  }

  confirmTranslation(translation: Translation, id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Put, this.api + '/' + id + '/translations/confirm', undefined, translation);
  }

  unConfirmTranslation(translation: Translation, id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Put, this.api + '/' + id + '/translations/unconfirm', undefined, translation);
  }

  delete(id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Delete, this.api, id, undefined);
  }

  updateStringComments(comments: Comment[], id: number) : Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Put, this.api + '/' + id + '/comments', undefined, comments);
  }

  createStringComment(comment: Comment, id: number, itemsOnPage: number, page: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Post, this.api + '/' + id + '/comments?itemsOnPage='+ itemsOnPage+'&page='+page, undefined, comment)
  }

  editStringComment(comment: Comment, id: number): Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Put, this.api + '/' + id + '/comments', undefined, comment)
  }

  getCommentsByStringId(id: number) : Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Get, this.api + '/' + id + '/comments', undefined);
  }

  getCommentsWithPagination(id: number, itemsOnPage: number, page: number) : Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Get, this.api + '/' + id + '/','paginatedComments?itemsOnPage='+itemsOnPage+'&page='+page);
  }

  deleteStringComment(commentId: string, id: number) : Observable<any> {
    return this.dataService.sendRequest(RequestMethod.Delete, this.api + '/' + id + '/comments/' + commentId, undefined);
  }

  getTranslationHistory(id: number, translationId: string, itemsOnPage: number, page: number) {
    return this.dataService.sendRequest(RequestMethod.Get, this.api + '/' + id + '/history/' + translationId, '?itemsOnPage=' + itemsOnPage+'&page='+page);
  }

  revertTranslationHistory(id: number, translationId: string, historyId: string) {
    return this.dataService.sendRequest(RequestMethod.Put, this.api + '/' + id + '/translations/','revert?translationId=' + translationId + '&historyId=' + historyId);
  }

  addOptionalTranslation(stringId, translationId, value) {
    return this.dataService.sendRequest(RequestMethod.Post, this.api + '/' + stringId + '/' + translationId, '/?value=' + value);
  }

  getOptionalTranslation(stringId, translationId) {
    return this.dataService.sendRequest(RequestMethod.Get, this.api + '/' + stringId + '/' + translationId, 'optional' );
  }

  changeStringStatus(id: number, groupName: string, status: boolean) {
    return this.dataService.sendRequest(RequestMethod.Get, this.api + '/' + id + '/status/' + status, '/?groupName=' + groupName);
  }
}
