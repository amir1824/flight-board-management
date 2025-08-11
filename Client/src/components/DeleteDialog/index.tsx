import styles from "./index.module.scss";
import { Button } from "@mui/material";
import type { FC } from "react";
import { DialogLayout } from "../DialogLayout";

interface DeleteDialogProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  hideSubTitle?: boolean;
  onDelete: () => void;
  flightNumber: string;
}

export const DeleteDialog: FC<DeleteDialogProps> = ({
  isOpen,
  onClose,
  title,
  flightNumber,
  onDelete,
}) => {
  const deleteItem = () => {
    onDelete();
    onClose();
  };

  return (
    <DialogLayout
      open={isOpen}
      onClose={onClose}
      dataCy="delete-dialog"
      title={title}
      subTitle={`Are you sure you want to delete flight ${flightNumber}?`}
    >
      <div className={styles.actions}>
        <Button
          variant="contained"
          color="primary"
          onClick={deleteItem}
          className={styles.deleteButton}
          data-cy="confirm-delete"
        >
          {"Yes"}
        </Button>
        <Button
          variant="contained"
          className={styles.cancelButton}
          onClick={onClose}
          data-cy="cancel-delete"
        >
          {"Cancel"}
        </Button>
      </div>
    </DialogLayout>
  );
};