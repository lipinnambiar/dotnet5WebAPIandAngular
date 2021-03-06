import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_model/Member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;

  constructor(
    private memberService: MembersService,
    private toster: ToastrService
  ) {}

  ngOnInit(): void {}

  addLikes(member: Member) {
    this.memberService.addLikes(member.username).subscribe(() => {
      this.toster.success('You liked ' + member.knownAs);
    });
  }
}
