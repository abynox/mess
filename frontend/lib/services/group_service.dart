import 'package:mess/models/group.dart';
import 'package:mess/models/member.dart';

class GroupService {
  static final GroupService instance = GroupService();

  List<Group> getGroups() {
    return [
      Group("Fakeuuid", "FakeGroup", [
        Member("uuid1", "Member1"),
        Member("uuid2", "Member2"),
        Member("uuid3", "Member3"),
      ]),
      Group("Fakeuuid2", "FakeGroup2", [Member("uuid1", "Member1"), Member("uuid2", "Member2")]),
    ];
  }
}

