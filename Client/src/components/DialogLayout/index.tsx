import { Dialog, DialogContent,  IconButton, type PaperProps, Typography } from "@mui/material"
import type { ReactNode, FC } from "react";
import {Close as CloseIcon} from "@mui/icons-material";
import styles from './index.module.scss'

interface DialogLayoutProps {
  children: ReactNode;
  open: boolean;
  onClose: () => void
  title?: string;
  className?: string;
  headerClassName?: string;
  closeIconColor?: string;
  subTitle?: string
  PaperProps?: Partial<PaperProps>
}

export const DialogLayout: FC<DialogLayoutProps> = ({
  children,
  open,
  onClose,
  title,
  subTitle,
  closeIconColor,

}: DialogLayoutProps) => {
  return (
    <>
      {open && <Dialog  open={open} >
        <div className={styles.header}>
          <IconButton className={styles.close} onClick={onClose}>
            <CloseIcon className={styles.closeIcon} stroke={closeIconColor ?? "#696969"} />
          </IconButton>
          {title && <Typography className={styles.title} variant="h6" >{title}</Typography>}
        </div>
        {subTitle && <Typography className={styles.subTitle} variant="body2" align="center">{subTitle}</Typography>}
        <DialogContent className={styles.content} >
          {children}
        </DialogContent>
      </Dialog >}
    </>
  );
};