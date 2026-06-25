import { useMemo, useState, type FormEvent } from "react"
import {
  Check,
  LoaderCircle,
  LogOut,
  Pencil,
  Plus,
  Trash2,
  UserMinus,
  UsersRound,
} from "lucide-react"
import { useNavigate } from "@tanstack/react-router"
import { toast } from "sonner"

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { ApiError } from "@/lib/api-client"
import {
  useAddGroupMembersMutation,
  useDeleteGroupConversationMutation,
  useGroupConversationDetailsQuery,
  useLeaveGroupConversationMutation,
  useRemoveGroupMemberMutation,
  useUpdateGroupConversationMutation,
  type GroupMember,
} from "@/lib/chat-api"
import { useAuthStore } from "@/lib/auth-store"
import { type FriendUser, useFriendsQuery } from "@/lib/friends-api"
import { cn } from "@/lib/utils"

type GroupSettingsDialogProps = {
  conversationId: string
  trigger?: React.ReactNode
  open?: boolean
  onOpenChange?: (open: boolean) => void
}

type ConfirmAction =
  | {
    type: "remove-member"
    member: GroupMember
  }
  | {
    type: "leave-group"
  }
  | {
    type: "delete-group"
  }

export const GroupSettingsDialog = ({
  conversationId,
  trigger,
  open: controlledOpen,
  onOpenChange,
}: GroupSettingsDialogProps) => {
  const [uncontrolledOpen, setUncontrolledOpen] = useState(false)
  const [nameDraft, setNameDraft] = useState<string | null>(null)
  const [avatarUrlDraft, setAvatarUrlDraft] = useState<string | null>(null)
  const [selectedFriendIds, setSelectedFriendIds] = useState<string[]>([])
  const [confirmAction, setConfirmAction] = useState<ConfirmAction | null>(
    null
  )
  const navigate = useNavigate()
  const currentUserId = useAuthStore((state) => state.user?.id)
  const open = controlledOpen ?? uncontrolledOpen

  const groupQuery = useGroupConversationDetailsQuery(conversationId, open)
  const friendsQuery = useFriendsQuery()
  const updateGroupMutation = useUpdateGroupConversationMutation()
  const addMembersMutation = useAddGroupMembersMutation()
  const removeMemberMutation = useRemoveGroupMemberMutation()
  const leaveGroupMutation = useLeaveGroupConversationMutation()
  const deleteGroupMutation = useDeleteGroupConversationMutation()

  const group = groupQuery.data
  const currentRole = group?.currentUserRole
  const canManageMembers = currentRole === "owner" || currentRole === "admin"
  const canDeleteGroup = currentRole === "owner"

  const existingMemberIds = useMemo(
    () => new Set(group?.members.map((member) => member.id) ?? []),
    [group?.members]
  )

  const availableFriends =
    friendsQuery.data
      ?.map((friend) => friend.user)
      .filter((friend) => !existingMemberIds.has(friend.id)) ?? []

  const handleOpenChange = (nextOpen: boolean) => {
    setUncontrolledOpen(nextOpen)
    onOpenChange?.(nextOpen)
    if (!nextOpen) {
      setNameDraft(null)
      setAvatarUrlDraft(null)
      setSelectedFriendIds([])
      setConfirmAction(null)
    }
  }

  const handleUpdateGroup = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (!group) {
      return
    }

    const nextName = (nameDraft ?? group.name).trim()
    const nextAvatarUrl = (avatarUrlDraft ?? group.avatarUrl ?? "").trim()

    if (!nextName) {
      return
    }

    updateGroupMutation.mutate(
      {
        conversationId,
        name: nextName,
        avatarUrl: nextAvatarUrl || null,
      },
      {
        onSuccess: () => {
          setNameDraft(null)
          setAvatarUrlDraft(null)
          toast.success("Group updated")
        },
        onError: showMutationError("Could not update group"),
      }
    )
  }

  const toggleFriend = (friendId: string) => {
    setSelectedFriendIds((current) =>
      current.includes(friendId)
        ? current.filter((id) => id !== friendId)
        : [...current, friendId]
    )
  }

  const handleAddMembers = () => {
    if (selectedFriendIds.length === 0) {
      return
    }

    addMembersMutation.mutate(
      {
        conversationId,
        memberUserIds: selectedFriendIds,
      },
      {
        onSuccess: () => {
          setSelectedFriendIds([])
          toast.success("Members added")
        },
        onError: showMutationError("Could not add members"),
      }
    )
  }

  const handleRemoveMember = (member: GroupMember) => {
    removeMemberMutation.mutate(
      {
        conversationId,
        memberUserId: member.id,
      },
      {
        onSuccess: () => toast.success("Member removed"),
        onError: showMutationError("Could not remove member"),
      }
    )
  }

  const handleLeaveGroup = () => {
    leaveGroupMutation.mutate(conversationId, {
      onSuccess: () => {
        handleOpenChange(false)
        toast.success("You left the group")
        void navigate({ to: "/" })
      },
      onError: showMutationError("Could not leave group"),
    })
  }

  const handleDeleteGroup = () => {
    deleteGroupMutation.mutate(conversationId, {
      onSuccess: () => {
        handleOpenChange(false)
        toast.success("Group dissolved")
        void navigate({ to: "/" })
      },
      onError: showMutationError("Could not dissolve group"),
    })
  }

  const handleConfirmAction = () => {
    if (!confirmAction) {
      return
    }

    const action = confirmAction
    setConfirmAction(null)

    if (action.type === "remove-member") {
      handleRemoveMember(action.member)
      return
    }

    if (action.type === "leave-group") {
      handleLeaveGroup()
      return
    }

    handleDeleteGroup()
  }

  return (
    <>
      <Dialog open={open} onOpenChange={handleOpenChange}>
        {trigger && <DialogTrigger asChild>{trigger}</DialogTrigger>}
        <DialogContent className="flex max-h-[calc(100dvh-2rem)] flex-col gap-0 overflow-hidden p-0 sm:max-w-lg">
          <div className="shrink-0 border-b border-border/60 px-5 pt-5 pb-4">
            <DialogTitle className="flex items-center gap-2 text-lg">
              <UsersRound className="size-5" />
              Group settings
            </DialogTitle>
            <DialogDescription>
              Manage group profile, members, and access.
            </DialogDescription>
          </div>

          <div className="min-h-0 flex-1 overflow-y-auto overscroll-contain px-5 pb-5">
            {groupQuery.isLoading && (
              <div className="flex items-center gap-2 py-8 text-sm text-muted-foreground">
                <LoaderCircle className="animate-spin" />
                Loading group
              </div>
            )}

            {groupQuery.isError && (
              <p className="py-8 text-sm text-muted-foreground">
                Could not load group settings.
              </p>
            )}

            {group && (
              <div className="space-y-6">
                <form className="space-y-3 pt-5" onSubmit={handleUpdateGroup}>
                  <div className="flex items-center gap-3">
                    <Avatar className="size-14">
                      <AvatarImage
                        src={
                          (avatarUrlDraft ?? group.avatarUrl ?? "") ||
                          undefined
                        }
                        alt={nameDraft ?? group.name}
                      />
                      <AvatarFallback>
                        {getFallback(nameDraft ?? group.name)}
                      </AvatarFallback>
                    </Avatar>
                    <div className="min-w-0 flex-1">
                      <p className="truncate font-semibold">{group.name}</p>
                      <p className="text-sm text-muted-foreground">
                        {group.members.length} members
                      </p>
                    </div>
                  </div>

                  <div className="space-y-1.5">
                    <label className="text-sm font-medium" htmlFor="groupName">
                      Name
                    </label>
                    <Input
                      id="groupName"
                      value={nameDraft ?? group.name}
                      onChange={(event) => setNameDraft(event.target.value)}
                      maxLength={100}
                      disabled={!canManageMembers}
                    />
                  </div>

                  <div className="space-y-1.5">
                    <label
                      className="text-sm font-medium"
                      htmlFor="groupAvatarUrl"
                    >
                      Avatar URL
                    </label>
                    <Input
                      id="groupAvatarUrl"
                      value={avatarUrlDraft ?? group.avatarUrl ?? ""}
                      onChange={(event) =>
                        setAvatarUrlDraft(event.target.value)
                      }
                      maxLength={2048}
                      disabled={!canManageMembers}
                    />
                  </div>

                  {canManageMembers && (
                    <Button
                      className="w-full h-12 rounded-full text-base"
                      disabled={
                        !(nameDraft ?? group.name).trim() ||
                        updateGroupMutation.isPending
                      }
                    >
                      {updateGroupMutation.isPending ? (
                        <LoaderCircle className="animate-spin" />
                      ) : (
                        <Pencil />
                      )}
                      Save changes
                    </Button>
                  )}
                </form>

                {canManageMembers && (
                  <section className="space-y-3 border-t border-border/60 pt-5 mb-0">
                    <div className="flex items-center justify-between gap-3">
                      <h3 className="font-semibold">Add members</h3>
                      <Button
                        type="button"
                        size="sm"
                        disabled={
                          selectedFriendIds.length === 0 ||
                          addMembersMutation.isPending
                        }
                        onClick={handleAddMembers}
                      >
                        {addMembersMutation.isPending ? (
                          <LoaderCircle className="animate-spin" />
                        ) : (
                          <Plus />
                        )}
                        Add
                      </Button>
                    </div>

                    <div className="max-h-48 overflow-y-auto">
                      {friendsQuery.isLoading && (
                        <StatusRow label="Loading friends" />
                      )}
                      {friendsQuery.isError && (
                        <p className="py-4 text-sm text-muted-foreground">
                          Could not load friends.
                        </p>
                      )}
                      {availableFriends.length === 0 && !friendsQuery.isLoading && (
                        <p className="py-4 text-sm text-muted-foreground">
                          No friends available to add.
                        </p>
                      )}
                      {availableFriends.map((friend) => (
                        <SelectableFriendRow
                          key={friend.id}
                          user={friend}
                          selected={selectedFriendIds.includes(friend.id)}
                          onToggle={() => toggleFriend(friend.id)}
                        />
                      ))}
                    </div>
                  </section>
                )}

                <section className="space-y-3 border-t border-border/60 pt-5 mb-0">
                  <h3 className="font-semibold">Members</h3>
                  <div>
                    {group.members.map((member) => {
                      const canRemove =
                        canManageMembers &&
                        member.id !== currentUserId &&
                        member.role !== "owner" &&
                        (currentRole === "owner" || member.role === "member")

                      return (
                        <MemberRow
                          key={member.id}
                          member={member}
                          trailing={
                            canRemove ? (
                              <Button
                                type="button"
                                variant="ghost"
                                size="icon-sm"
                                aria-label={`Remove ${member.fullName || member.username}`}
                                disabled={removeMemberMutation.isPending}
                                onClick={() =>
                                  setConfirmAction({
                                    type: "remove-member",
                                    member,
                                  })
                                }
                              >
                                <UserMinus />
                              </Button>
                            ) : null
                          }
                        />
                      )
                    })}
                  </div>
                </section>

                <section className="space-y-2 border-t border-border/60 pt-5">
                  {currentRole !== "owner" && (
                    <Button
                      type="button"
                      variant="destructive"
                      className="w-full h-12 rounded-full text-base"
                      disabled={leaveGroupMutation.isPending}
                      onClick={() => setConfirmAction({ type: "leave-group" })}
                    >
                      {leaveGroupMutation.isPending ? (
                        <LoaderCircle className="animate-spin" />
                      ) : (
                        <LogOut />
                      )}
                      Leave group
                    </Button>
                  )}

                  {canDeleteGroup && (
                    <Button
                      type="button"
                      variant="destructive"
                      className="w-full h-12 rounded-full text-base"
                      disabled={deleteGroupMutation.isPending}
                      onClick={() => setConfirmAction({ type: "delete-group" })}
                    >
                      {deleteGroupMutation.isPending ? (
                        <LoaderCircle className="animate-spin" />
                      ) : (
                        <Trash2 />
                      )}
                      Dissolve group
                    </Button>
                  )}
                </section>
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>

      <ConfirmGroupActionDialog
        action={confirmAction}
        onOpenChange={(nextOpen) => {
          if (!nextOpen) {
            setConfirmAction(null)
          }
        }}
        onConfirm={handleConfirmAction}
      />
    </>
  )
}

const ConfirmGroupActionDialog = ({
  action,
  onOpenChange,
  onConfirm,
}: {
  action: ConfirmAction | null
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
}) => {
  const copy = getConfirmCopy(action)

  return (
    <Dialog open={action !== null} onOpenChange={onOpenChange}>
      <DialogContent className="gap-4 sm:max-w-sm">
        <div className="space-y-2 pr-8">
          <DialogTitle>{copy.title}</DialogTitle>
          <DialogDescription>{copy.description}</DialogDescription>
        </div>

        <DialogFooter className="grid grid-cols-2 gap-2 sm:grid-cols-2">
          <Button
            type="button"
            variant="outline"
            className="w-full h-12 rounded-full text-base"
            onClick={() => onOpenChange(false)}
          >
            Cancel
          </Button>
          <Button
            type="button"
            variant="destructive"
            className="w-full h-12 rounded-full text-base"
            onClick={onConfirm}
          >
            {copy.confirmLabel}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

const MemberRow = ({
  member,
  trailing,
}: {
  member: GroupMember
  trailing?: React.ReactNode
}) => {
  const name = member.fullName || member.username

  return (
    <div className="flex min-w-0 items-center gap-3 border-t border-border/60 py-3 first:border-t-0">
      <Avatar className="size-10">
        <AvatarImage src={member.avatarUrl ?? undefined} alt={name} />
        <AvatarFallback>{getFallback(name)}</AvatarFallback>
      </Avatar>

      <div className="min-w-0 flex-1">
        <p className="truncate font-medium">{name}</p>
        <p className="truncate text-xs text-muted-foreground">
          @{member.username} · {formatRole(member.role)}
        </p>
      </div>

      {trailing}
    </div>
  )
}

const SelectableFriendRow = ({
  user,
  selected,
  onToggle,
}: {
  user: FriendUser
  selected: boolean
  onToggle: () => void
}) => {
  const name = user.fullName || user.username

  return (
    <button
      type="button"
      className="flex w-full min-w-0 items-center gap-3 border-t border-border/60 py-3 text-left first:border-t-0"
      onClick={onToggle}
    >
      <Avatar className="size-10">
        <AvatarImage src={user.avatarUrl ?? undefined} alt={name} />
        <AvatarFallback>{getFallback(name)}</AvatarFallback>
      </Avatar>

      <div className="min-w-0 flex-1">
        <p className="truncate font-medium">{name}</p>
        <p className="truncate text-xs text-muted-foreground">
          @{user.username}
        </p>
      </div>

      <span
        className={cn(
          "flex size-6 items-center justify-center rounded-full border",
          selected && "border-primary bg-primary text-gray-900"
        )}
      >
        {selected && <Check className="size-4" />}
      </span>
    </button>
  )
}

const StatusRow = ({ label }: { label: string }) => (
  <div className="flex items-center gap-2 py-4 text-sm text-muted-foreground">
    <LoaderCircle className="animate-spin" />
    {label}
  </div>
)

const showMutationError = (fallback: string) => (error: unknown) => {
  toast.error(error instanceof ApiError ? error.message : fallback)
}

const getConfirmCopy = (action: ConfirmAction | null) => {
  if (action?.type === "remove-member") {
    const name = action.member.fullName || action.member.username

    return {
      title: "Remove member?",
      description: `${name} will lose access to this group conversation.`,
      confirmLabel: "Remove",
    }
  }

  if (action?.type === "leave-group") {
    return {
      title: "Leave group?",
      description:
        "You will stop receiving messages from this group and it will be removed from your chat list.",
      confirmLabel: "Leave",
    }
  }

  if (action?.type === "delete-group") {
    return {
      title: "Dissolve group?",
      description:
        "This group will be removed for every member. This action cannot be undone.",
      confirmLabel: "Dissolve",
    }
  }

  return {
    title: "Are you sure?",
    description: "Please confirm before continuing.",
    confirmLabel: "Confirm",
  }
}

const formatRole = (role: GroupMember["role"]) => {
  switch (role) {
    case "owner":
      return "Owner"
    case "admin":
      return "Admin"
    default:
      return "Member"
  }
}

const getFallback = (name: string) => {
  return name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase()
}
