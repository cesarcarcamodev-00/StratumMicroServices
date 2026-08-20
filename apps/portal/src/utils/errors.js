export const getErrorMessage = (err) => {
  const data = err?.response?.data;
  if (data?.errors?.length) return data.errors[0];
  if (data?.message) return data.message;
  if (err?.response?.status === 403) return 'No tienes permisos para realizar esta acción';
  return 'Ha ocurrido un error';
};
