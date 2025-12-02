import 'package:mess/models/user.dart';

/// A member is a Person that member of a group
/// That Person may or may not have a real user account associated to them
final class Member {
  final String uuid;
  final String name;
  final User? associatedUser;

  Member(this.uuid, this.name, {this.associatedUser});
}
