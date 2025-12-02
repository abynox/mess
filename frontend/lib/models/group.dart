import 'package:mess/models/member.dart';

final class Group {
  final String uuid;
  final String name;
  final List<Member> members;

  Group(this.uuid, this.name, this.members);
}