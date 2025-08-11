import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import dayjs from "dayjs";
import type { CreateFlightDto } from "../../store/dbModels/FlightModel";
import { useAddFlight } from "../../api/flightsQueries";
import { TextField, Button, Paper, Typography, Stack } from "@mui/material";
import styles from "./index.module.scss";
import { getApiErrorMessage } from "../../utils/apiError";

type CreateFlightForm = {
  flightNumber: string;
  destination: string;
  departure: string;
  gate?: string;
};

const fmtLocal = (d = new Date()) =>
  `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(
    d.getDate()
  ).padStart(2, "0")}T${String(d.getHours()).padStart(2, "0")}:${String(
    d.getMinutes()
  ).padStart(2, "0")}`;

const schema: yup.ObjectSchema<CreateFlightForm> = yup.object({
  flightNumber: yup
    .string()
    .trim()
    .required("Flight # is required")
    .matches(/^[A-Z]{1,3}\d{1,4}$/i, "e.g. LY123"),
  destination: yup
    .string()
    .trim()
    .required("Destination is required")
    .min(2)
    .max(50),
  departure: yup
    .string()
    .required("Departure time is required")
    .test(
      "is-valid-datetime",
      "Invalid date/time",
      (v) => !!v && dayjs(v).isValid()
    )
    .test(
      "not-in-past",
      "Must be in the future",
      (v) => !!v && dayjs(v).isAfter(dayjs().subtract(1, "minute"))
    ),
  gate: yup.string().trim().max(10).optional(),
});

export default function FlightsForm({
  onAfterSubmit,
}: {
  onAfterSubmit?: () => void;
}) {
  const add = useAddFlight();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting, isValid },
    reset,
  } = useForm<CreateFlightForm>({
    resolver: yupResolver(schema),
    mode: "onChange",
    defaultValues: {
      flightNumber: "",
      destination: "",
      gate: "",
      departure: fmtLocal(new Date(Date.now() + 2 * 60 * 60 * 1000)), 
    },
  });

  const onSubmit = (values: CreateFlightForm) => {
    const dto: CreateFlightDto = {
      flightNumber: values.flightNumber.trim(),
      destination: values.destination.trim(),
      gate: values.gate?.trim() || "",
      departureTime: dayjs(values.departure).toISOString(),
      status: "Scheduled",
    };

    add.mutate(dto, {
      onSuccess: () => {
        reset({
          flightNumber: "",
          destination: "",
          gate: "",
          departure: fmtLocal(new Date(Date.now() + 2 * 60 * 60 * 1000)),
        });
        onAfterSubmit?.();
      },
    });
  };

  return (
    <Paper className={styles.formContainer} elevation={0}>
      <Typography variant="h6" className={styles.title}>
        Add New Flight
      </Typography>

      <form onSubmit={handleSubmit(onSubmit)} className={styles.formBody}>
        <Stack spacing={1.5}>
          <TextField
            label="Flight #"
            size="small"
            {...register("flightNumber")}
            error={!!errors.flightNumber}
            helperText={errors.flightNumber?.message}
          />

          <TextField
            label="Destination"
            size="small"
            {...register("destination")}
            error={!!errors.destination}
            helperText={errors.destination?.message}
          />

          <TextField
            label="Departure"
            type="datetime-local"
            size="small"
            {...register("departure")}
            error={!!errors.departure}
            helperText={errors.departure?.message}
            InputLabelProps={{ shrink: true }}
          />

          <TextField
            label="Gate"
            size="small"
            {...register("gate")}
            error={!!errors.gate}
            helperText={errors.gate?.message}
          />

          <Button
            type="submit"
            variant="contained"
            disabled={!isValid || isSubmitting || add.isPending}
          >
            {add.isPending ? "Adding…" : "Add Flight"}
          </Button>

          {add.isError && (
            <Typography color="error" variant="body2">
              {getApiErrorMessage(add.error)}
            </Typography>
          )}
        </Stack>
      </form>
    </Paper>
  );
}