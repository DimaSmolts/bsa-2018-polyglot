import { Component, OnInit, Input, ViewChild, ElementRef} from '@angular/core';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { UserProfile } from '../../../models/user-profile';
import { UserService } from '../../../services/user.service';
import { RatingService } from '../../../services/rating.service';
import { Rating } from '../../../models';
import { StarRatingColor } from '../star-rating/star-rating.component';

@Component({
  selector: 'app-tab-review',
  templateUrl: './tab-review.component.html',
  styleUrls: ['./tab-review.component.sass']
})
export class TabReviewComponent implements OnInit {
  rating:number = 1;
  starCount:number = 5;
  starColor:StarRatingColor = StarRatingColor.primary;
  starColorP:StarRatingColor = StarRatingColor.accent;
  starColorW:StarRatingColor = StarRatingColor.warn;

  @Input()  public userProfile: UserProfile;
  reviewForm = this.fb.group({
    reviewBody: ['', Validators.required]
    });

    @ViewChild('textArea') textArea: ElementRef;

  constructor(private fb: FormBuilder,
    private userService: UserService,
    private ratingsService: RatingService) { }

  ngOnInit() {
  }

  onRatingChanged(rating){
    console.log(rating);
    this.rating = rating;
  }
  
  addReview(reviewBody: string, rating: number){
    const ratingObj = {
      comment: reviewBody,
      rate: rating,
      createdById: this.userService.getCurrrentUser().id,
      userId: this.userProfile.id,
      createdAt: new Date(Date.now())
    };

    this.ratingsService.create(ratingObj).subscribe(
      (data) => {
        this.reviewForm.reset();
          if (data) {
            this.userProfile.ratings.unshift(data);
            console.log("Review added.");
          }
          else {
            console.log("Review not added.");
          }
      },
      err => {
        console.log("Error", err);
      });
  }
}